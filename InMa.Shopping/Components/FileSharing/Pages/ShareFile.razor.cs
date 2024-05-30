using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;

namespace InMa.Shopping.Components.FileSharing.Pages;

public partial class ShareFile
{
    [Inject] private IFilesRepository FilesRepository { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    private readonly long _maxFileSizeInBytes = 5L * 1_024 * 1_024 * 1_024;

    public SharedFileVm SharedFilesVm { get; set; } = new() {DateCaptured = new DateTime(2022, 2, 3)};
    private IBrowserFile[] _inputFiles = [];
    
    FluentInputFile? myFileByStream = default!;
    int? progressPercent;
    string? progressTitle;

    //List<string> Files = new();

    int ProgressPercent = 0;
    
    void OnFilesSelectedAsync(InputFileChangeEventArgs file)
    {
        try
        {
            _inputFiles = file.GetMultipleFiles().ToArray();

            SharedFilesVm.FileProperties =
                _inputFiles.Select((f, i) =>
                        new SharedFileInputProperties
                        {
                            Name = f.Name,
                            OriginalName = f.Name,
                            ContentType = f.ContentType,
                            FileSizeBytes = f.Size,
                            LastModified = f.LastModified
                        })
                    .ToArray();

            progressPercent = 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }

    async Task UploadAllButtonClicked(MouseEventArgs args)
    {
        try
        {
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            progressPercent = 0;
            progressTitle = "uploading files";

            for (int i = 0; i < _inputFiles.Length; i++)
            {
                var browserFile = _inputFiles[i];
                var fileProperties = SharedFilesVm.FileProperties[i];

                var blobId = await FilesRepository.UploadFile(
                    browserFile.OpenReadStream(_maxFileSizeInBytes, CancellationToken.None),
                    new UploadFileInfo 
                    {
                        FileName = fileProperties.Name,
                        OriginalName = fileProperties.OriginalName,
                        ContentType = fileProperties.ContentType,
                        LastModified = fileProperties.LastModified,
                        FileSizeBytes = fileProperties.FileSizeBytes,
                        UploaderEmail = state.User.Identity!.Name!,
                        CountryCode = SharedFilesVm.CountryCode,
                        Region = SharedFilesVm.Region,
                        City = SharedFilesVm.City,
                        DateCaptured = SharedFilesVm.DateCaptured.GetValueOrDefault(DateTime.UtcNow),
                        Tags = SharedFilesVm.Tags.Split(';').ToArray(),
                        SharedFileUsers = SharedFilesVm.ShareWith.Split(';')
                    },
                    CancellationToken.None);
                progressPercent++;
            }

            progressTitle = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }
    
    async Task UploadFileClicked(IBrowserFile browserFile, int index)
    {
        
    }

    void OnCompleted(IEnumerable<FluentInputFileEventArgs> files)
    {
        progressPercent = myFileByStream!.ProgressPercent;
        progressTitle = myFileByStream!.ProgressTitle;
    }
}