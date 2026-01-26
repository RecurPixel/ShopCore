namespace ShopCore.Api.Files;

public class FormFileAdapter : IFile
{
    private readonly IFormFile _file;

    public FormFileAdapter(IFormFile file)
    {
        _file = file;
    }

    public string FileName => _file.FileName;
    public string ContentType => _file.ContentType;
    public long Length => _file.Length;

    public Stream OpenReadStream()
        => _file.OpenReadStream();
}
