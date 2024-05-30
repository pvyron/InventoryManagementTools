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

    private SharedFileVm _sharedFileVm = new();
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

            _sharedFileVm.FileProperties =
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
                var fileProperties = _sharedFileVm.FileProperties[i];
                
                var blobId = await FilesRepository.UploadFile(
                    browserFile.OpenReadStream(), new UploadFileInfo()
                    {
                        FileName = fileProperties.Name,
                        OriginalName = fileProperties.OriginalName,
                        ContentType = fileProperties.ContentType,
                        LastModified = fileProperties.LastModified,
                        FileSizeBytes = fileProperties.FileSizeBytes,
                        UploaderEmail = state.User.Identity!.Name!,
                        CountryCode = _sharedFileVm.CountryCode,
                        Region = _sharedFileVm.Region,
                        City = _sharedFileVm.City,
                        DateCaptured = _sharedFileVm.DateCaptured,
                        Tags = _sharedFileVm.Tags.Split(';').ToArray(),
                        SharedFileUsers = _sharedFileVm.ShareWith.Split(';')
                    }, CancellationToken.None);
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