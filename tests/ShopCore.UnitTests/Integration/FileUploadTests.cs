using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ShopCore.Application.Categories.DTOs;
using ShopCore.Application.Common.Interfaces;
using ShopCore.Application.Products.DTOs;
using ShopCore.UnitTests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;

namespace ShopCore.UnitTests.Integration;

/// <summary>
/// WebApplicationFactory that replaces IFileStorageService with a mock
/// so file upload tests don't write to disk or hit external storage.
/// </summary>
public class FileUploadWebApplicationFactory : ShopCoreWebApplicationFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            // Remove all registered IFileStorageService implementations
            ServiceDescriptor[] storageDescriptors = services
                .Where(d => d.ServiceType == typeof(IFileStorageService))
                .ToArray();

            foreach (ServiceDescriptor d in storageDescriptors)
                services.Remove(d);

            // Register a mock that returns predictable URLs without disk I/O
            Mock<IFileStorageService> mockStorage = new();

            mockStorage
                .Setup(s => s.UploadFileAsync(
                    It.IsAny<IFile>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((IFile _, string fileName, CancellationToken _) =>
                    $"https://storage.test/{fileName}");

            mockStorage
                .Setup(s => s.UploadFileAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Stream _, string fileName, string _, CancellationToken _) =>
                    $"https://storage.test/{fileName}");

            mockStorage
                .Setup(s => s.DeleteFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            mockStorage
                .Setup(s => s.FileExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            mockStorage
                .Setup(s => s.GetFileUrl(It.IsAny<string>()))
                .Returns((string path) => $"https://storage.test/{path}");

            services.AddScoped(_ => mockStorage.Object);
        });
    }
}

/// <summary>
/// Integration tests for all API endpoints that handle file uploads (images, photos, logos).
/// </summary>
public class FileUploadTests : IntegrationTestBase, IClassFixture<FileUploadWebApplicationFactory>
{
    public FileUploadTests(FileUploadWebApplicationFactory factory) : base(factory) { }

    #region File content helpers

    /// <summary>Creates a multipart form with one image file.</summary>
    private static MultipartFormDataContent CreateSingleFileContent(
        string fieldName = "file",
        string fileName = "test.jpg",
        string contentType = "image/jpeg",
        int sizeBytes = 1024)
    {
        byte[] imageBytes = CreateFakeImageBytes(sizeBytes);
        ByteArrayContent fileContent = new(imageBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        MultipartFormDataContent form = new();
        form.Add(fileContent, fieldName, fileName);
        return form;
    }

    /// <summary>Creates a multipart form with multiple image files.</summary>
    private static MultipartFormDataContent CreateMultiFileContent(
        string fieldName = "files",
        int count = 3,
        string contentType = "image/jpeg")
    {
        MultipartFormDataContent form = new();

        for (int i = 0; i < count; i++)
        {
            byte[] imageBytes = CreateFakeImageBytes(512 + i * 256);
            ByteArrayContent fileContent = new(imageBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            form.Add(fileContent, fieldName, $"image_{i + 1}.jpg");
        }

        return form;
    }

    /// <summary>Creates a minimal fake JPEG (starts with JPEG magic bytes).</summary>
    private static byte[] CreateFakeImageBytes(int size)
    {
        byte[] bytes = new byte[size];
        // JPEG magic bytes
        bytes[0] = 0xFF;
        bytes[1] = 0xD8;
        bytes[2] = 0xFF;
        bytes[3] = 0xE0;
        return bytes;
    }

    /// <summary>Creates a minimal fake PNG (starts with PNG magic bytes).</summary>
    private static byte[] CreateFakePngBytes(int size = 1024)
    {
        byte[] bytes = new byte[size];
        // PNG magic bytes
        bytes[0] = 0x89;
        bytes[1] = 0x50; // 'P'
        bytes[2] = 0x4E; // 'N'
        bytes[3] = 0x47; // 'G'
        return bytes;
    }

    #endregion

    // ============================================================
    // POST /api/v1/users/me/avatar — Upload user avatar
    // ============================================================

    #region User Avatar Upload

    [Fact]
    public async Task UploadAvatar_WithValidJpeg_ReturnsOkWithUrl()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "avatar.jpg", "image/jpeg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/users/me/avatar", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string url = await response.Content.ReadAsStringAsync();
        url.Should().NotBeNullOrEmpty();
        url.Should().Contain("https://storage.test/");
    }

    [Fact]
    public async Task UploadAvatar_WithValidPng_ReturnsOkWithUrl()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        byte[] pngBytes = CreateFakePngBytes();
        ByteArrayContent fileContent = new(pngBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

        MultipartFormDataContent form = new();
        form.Add(fileContent, "file", "avatar.png");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/users/me/avatar", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string url = await response.Content.ReadAsStringAsync();
        url.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UploadAvatar_WithNoFile_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();
        MultipartFormDataContent form = new(); // empty — no file attached

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/users/me/avatar", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadAvatar_WithEmptyFile_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();

        ByteArrayContent emptyContent = new(Array.Empty<byte>());
        emptyContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        MultipartFormDataContent form = new();
        form.Add(emptyContent, "file", "empty.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/users/me/avatar", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadAvatar_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthentication();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "avatar.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/users/me/avatar", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UploadAvatar_WithLargeFile_ReturnsOk()
    {
        // Arrange - 5 MB file
        await AuthenticateAsCustomerAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "large_avatar.jpg", "image/jpeg", sizeBytes: 5 * 1024 * 1024);

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/users/me/avatar", form);

        // Assert — upload should succeed; size limits are enforced at infrastructure level
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UploadAvatar_VendorCanAlsoUpload_ReturnsOk()
    {
        // Arrange — avatar upload is not role-restricted; any authenticated user can upload
        await AuthenticateAsVendorAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "vendor_avatar.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/users/me/avatar", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string url = await response.Content.ReadAsStringAsync();
        url.Should().NotBeNullOrEmpty();
    }

    #endregion

    // ============================================================
    // POST /api/v1/vendors/me/logo — Upload vendor logo
    // ============================================================

    #region Vendor Logo Upload

    [Fact]
    public async Task UploadVendorLogo_WithValidJpeg_ReturnsOkWithUrl()
    {
        // Arrange
        await AuthenticateAsVendorAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "logo.jpg", "image/jpeg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/logo", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string url = await response.Content.ReadAsStringAsync();
        url.Should().NotBeNullOrEmpty();
        url.Should().Contain("https://storage.test/");
    }

    [Fact]
    public async Task UploadVendorLogo_WithValidPng_ReturnsOkWithUrl()
    {
        // Arrange
        await AuthenticateAsVendorAsync();

        byte[] pngBytes = CreateFakePngBytes();
        ByteArrayContent fileContent = new(pngBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

        MultipartFormDataContent form = new();
        form.Add(fileContent, "file", "logo.png");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/logo", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string url = await response.Content.ReadAsStringAsync();
        url.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UploadVendorLogo_WithNoFile_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsVendorAsync();
        MultipartFormDataContent form = new();

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/logo", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadVendorLogo_WithEmptyFile_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsVendorAsync();

        ByteArrayContent emptyContent = new(Array.Empty<byte>());
        emptyContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        MultipartFormDataContent form = new();
        form.Add(emptyContent, "file", "empty.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/logo", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadVendorLogo_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthentication();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "logo.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/logo", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UploadVendorLogo_AsCustomer_ReturnsForbidden()
    {
        // Arrange — logo upload requires Vendor role
        await AuthenticateAsCustomerAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "logo.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/logo", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    // ============================================================
    // POST /api/v1/vendors/me/products/{id}/images — Upload product images
    // ============================================================

    #region Product Images Upload

    [Fact]
    public async Task UploadProductImages_WithSingleValidImage_ReturnsOkWithUrls()
    {
        // Arrange
        await AuthenticateAsVendorAsync();
        int productId = await GetFirstVendorProductIdAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("files", "product.jpg", "image/jpeg");

        // Act
        HttpResponseMessage response = await Client.PostAsync($"/api/v1/vendors/me/products/{productId}/images", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        List<ProductImageDto>? images = await DeserializeAsync<List<ProductImageDto>>(response);
        images.Should().NotBeNullOrEmpty();
        images!.First().ImageUrl.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UploadProductImages_WithMultipleImages_ReturnsAllUrls()
    {
        // Arrange
        await AuthenticateAsVendorAsync();
        int productId = await GetFirstVendorProductIdAsync();
        using MultipartFormDataContent form = CreateMultiFileContent("files", count: 3);

        // Act
        HttpResponseMessage response = await Client.PostAsync($"/api/v1/vendors/me/products/{productId}/images", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        List<ProductImageDto>? images = await DeserializeAsync<List<ProductImageDto>>(response);
        images.Should().NotBeNullOrEmpty();
        images!.Count.Should().Be(3);
        images.Should().OnlyContain(i => !string.IsNullOrEmpty(i.ImageUrl));
    }

    [Fact]
    public async Task UploadProductImages_WithNoFiles_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsVendorAsync();
        int productId = await GetFirstVendorProductIdAsync();
        MultipartFormDataContent form = new();

        // Act
        HttpResponseMessage response = await Client.PostAsync($"/api/v1/vendors/me/products/{productId}/images", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadProductImages_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthentication();
        using MultipartFormDataContent form = CreateSingleFileContent("files", "product.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/products/1/images", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UploadProductImages_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsCustomerAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("files", "product.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/products/1/images", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UploadProductImages_WithPngFormat_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsVendorAsync();
        int productId = await GetFirstVendorProductIdAsync();

        byte[] pngBytes = CreateFakePngBytes();
        ByteArrayContent fileContent = new(pngBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

        MultipartFormDataContent form = new();
        form.Add(fileContent, "files", "product.png");

        // Act
        HttpResponseMessage response = await Client.PostAsync($"/api/v1/vendors/me/products/{productId}/images", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UploadProductImages_ToNonExistentProduct_ReturnsNotFoundOrForbidden()
    {
        // Arrange
        await AuthenticateAsVendorAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("files", "product.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/products/999999/images", form);

        // Assert — product doesn't belong to vendor or doesn't exist
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.Forbidden, HttpStatusCode.BadRequest);
    }

    #endregion

    // ============================================================
    // POST /api/v1/categories/{id}/image — Upload category image (Admin only)
    // ============================================================

    #region Category Image Upload

    [Fact]
    public async Task UploadCategoryImage_AsAdmin_ReturnsOkWithUrl()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        int categoryId = await GetFirstCategoryIdAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "category.jpg", "image/jpeg");

        // Act
        HttpResponseMessage response = await Client.PostAsync($"/api/v1/categories/{categoryId}/image", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string url = await response.Content.ReadAsStringAsync();
        url.Should().NotBeNullOrEmpty();
        url.Should().Contain("https://storage.test/");
    }

    [Fact]
    public async Task UploadCategoryImage_AsAdmin_WithPng_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        int categoryId = await GetFirstCategoryIdAsync();

        byte[] pngBytes = CreateFakePngBytes();
        ByteArrayContent fileContent = new(pngBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

        MultipartFormDataContent form = new();
        form.Add(fileContent, "file", "category.png");

        // Act
        HttpResponseMessage response = await Client.PostAsync($"/api/v1/categories/{categoryId}/image", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UploadCategoryImage_WithNoFile_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        int categoryId = await GetFirstCategoryIdAsync();
        MultipartFormDataContent form = new();

        // Act
        HttpResponseMessage response = await Client.PostAsync($"/api/v1/categories/{categoryId}/image", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadCategoryImage_WithEmptyFile_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        int categoryId = await GetFirstCategoryIdAsync();

        ByteArrayContent emptyContent = new(Array.Empty<byte>());
        emptyContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        MultipartFormDataContent form = new();
        form.Add(emptyContent, "file", "empty.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync($"/api/v1/categories/{categoryId}/image", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadCategoryImage_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthentication();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "category.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/categories/1/image", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UploadCategoryImage_AsVendor_ReturnsForbidden()
    {
        // Arrange — category image upload requires Admin role
        await AuthenticateAsVendorAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "category.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/categories/1/image", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UploadCategoryImage_AsCustomer_ReturnsForbidden()
    {
        // Arrange — category image upload requires Admin role
        await AuthenticateAsCustomerAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "category.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/categories/1/image", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UploadCategoryImage_ForNonExistentCategory_ReturnsNotFound()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "category.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/categories/999999/image", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    // ============================================================
    // POST /api/v1/vendors/me/deliveries/{id}/photo — Upload delivery photo
    // ============================================================

    #region Delivery Photo Upload

    [Fact]
    public async Task UploadDeliveryPhoto_WithNoFile_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsVendorAsync();
        MultipartFormDataContent form = new();

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/deliveries/1/photo", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadDeliveryPhoto_WithEmptyFile_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsVendorAsync();

        ByteArrayContent emptyContent = new(Array.Empty<byte>());
        emptyContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        MultipartFormDataContent form = new();
        form.Add(emptyContent, "file", "empty.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/deliveries/1/photo", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadDeliveryPhoto_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthentication();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "delivery.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/deliveries/1/photo", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UploadDeliveryPhoto_AsCustomer_ReturnsForbidden()
    {
        // Arrange — delivery photo upload requires Vendor role
        await AuthenticateAsCustomerAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "delivery.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/deliveries/1/photo", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UploadDeliveryPhoto_ForNonExistentDelivery_ReturnsNotFoundOrBadRequest()
    {
        // Arrange — delivery ID 999999 does not exist
        await AuthenticateAsVendorAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "delivery.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/deliveries/999999/photo", form);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    #endregion

    // ============================================================
    // Cross-cutting: content-type and edge-case tests
    // ============================================================

    #region Edge Cases

    [Fact]
    public async Task UploadAvatar_WithNonImageContentType_IsAcceptedOrRejected()
    {
        // Arrange — plain text file sent as "image"; server may accept or reject based on business rules
        await AuthenticateAsCustomerAsync();

        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes("this is not an image");
        ByteArrayContent fileContent = new(textBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

        MultipartFormDataContent form = new();
        form.Add(fileContent, "file", "not_an_image.txt");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/users/me/avatar", form);

        // Assert — content may pass (storage is mocked) or fail with bad request if validated at app layer
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async Task UploadProductImages_MixedImageFormats_ReturnsOkWithAllUrls()
    {
        // Arrange — one JPEG and one PNG in the same request
        await AuthenticateAsVendorAsync();
        int productId = await GetFirstVendorProductIdAsync();

        MultipartFormDataContent form = new();

        byte[] jpegBytes = CreateFakeImageBytes(512);
        ByteArrayContent jpegContent = new(jpegBytes);
        jpegContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        form.Add(jpegContent, "files", "photo.jpg");

        byte[] pngBytes = CreateFakePngBytes(512);
        ByteArrayContent pngContent = new(pngBytes);
        pngContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        form.Add(pngContent, "files", "photo.png");

        // Act
        HttpResponseMessage response = await Client.PostAsync($"/api/v1/vendors/me/products/{productId}/images", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        List<ProductImageDto>? images = await DeserializeAsync<List<ProductImageDto>>(response);
        images!.Count.Should().Be(2);
        images.Should().OnlyContain(i => !string.IsNullOrEmpty(i.ImageUrl));
    }

    [Fact]
    public async Task UploadVendorLogo_VerifyReturnedUrlIsValidFormat()
    {
        // Arrange
        await AuthenticateAsVendorAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "logo.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync("/api/v1/vendors/me/logo", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string url = await response.Content.ReadAsStringAsync();
        url.Should().NotBeNullOrEmpty();
        Uri.TryCreate(url, UriKind.Absolute, out Uri? uri).Should().BeTrue("returned value should be a valid absolute URL");
        uri!.Scheme.Should().BeOneOf("http", "https");
    }

    [Fact]
    public async Task UploadCategoryImage_VerifyReturnedUrlIsValidFormat()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        int categoryId = await GetFirstCategoryIdAsync();
        using MultipartFormDataContent form = CreateSingleFileContent("file", "category.jpg");

        // Act
        HttpResponseMessage response = await Client.PostAsync($"/api/v1/categories/{categoryId}/image", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string url = await response.Content.ReadAsStringAsync();
        Uri.TryCreate(url, UriKind.Absolute, out Uri? uri).Should().BeTrue();
        uri!.Scheme.Should().BeOneOf("http", "https");
    }

    #endregion

    // ============================================================
    // Private lookup helpers
    // ============================================================

    private async Task<int> GetFirstVendorProductIdAsync()
    {
        HttpResponseMessage response = await Client.GetAsync("/api/v1/vendors/me/products");
        response.EnsureSuccessStatusCode();

        PagedResponse<ProductDto>? paged = await DeserializeAsync<PagedResponse<ProductDto>>(response);
        paged.Should().NotBeNull();
        paged!.Items.Should().NotBeEmpty("vendor must have at least one product to run this test");
        return paged.Items.First().Id;
    }

    private async Task<int> GetFirstCategoryIdAsync()
    {
        HttpResponseMessage response = await Client.GetAsync("/api/v1/categories");
        response.EnsureSuccessStatusCode();

        PagedResponse<CategoryDto>? paged = await DeserializeAsync<PagedResponse<CategoryDto>>(response);
        paged.Should().NotBeNull();
        paged!.Items.Should().NotBeEmpty("at least one category must exist to run this test");
        return paged.Items.First().Id;
    }
}