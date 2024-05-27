using InMa.Shopping.Data.Repositories.Abstractions;
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
    
    FluentInputFile? myFileByStream = default!;
    int? progressPercent;
    string? progressTitle;

    //List<string> Files = new();

    int ProgressPercent = 0;
    (IBrowserFile, int)[] browserFiles = Array.Empty<(IBrowserFile, int)>();
    
    void OnFilesSelectedAsync(InputFileChangeEventArgs file)
    {
        try
        {
            browserFiles = file.GetMultipleFiles().Select((f, i) => (f, i)).ToArray();
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
            foreach (var (file, index) in browserFiles)
            {
                var blobId = await FilesRepository.UploadFile(
                    file.OpenReadStream(), new UploadFileInfo()
                    {
                        FileName = "testfile",
                        SharedFileUsers = Enumerable.Empty<string>(),
                        UploaderEmail = state.User.Identity!.Name!
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