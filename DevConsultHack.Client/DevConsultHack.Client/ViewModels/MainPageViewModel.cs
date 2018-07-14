using DevConsultHack.Client.Repositories;
using DevConsultHack.Client.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace DevConsultHack.Client.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IMediaService _mediaService;
        private readonly IPictureRepository _pictureRepository;
        private readonly IPageDialogService _pageDialogService;
        private DelegateCommand _takePhotoCommand;
        public DelegateCommand TakePhotoCommand =>
            _takePhotoCommand ?? (_takePhotoCommand = new DelegateCommand(ExecuteTakePhotoCommand));

        private ImageSource _latestPicture;
        public ImageSource LatestPicture
        {
            get { return _latestPicture; }
            set { SetProperty(ref _latestPicture, value); }
        }

        private string _emotionResult;
        public string EmotionResult
        {
            get { return _emotionResult; }
            set { SetProperty(ref _emotionResult, value, () => RaisePropertyChanged(nameof(HasEmotionResult))); }
        }

        public bool HasEmotionResult => !string.IsNullOrEmpty(EmotionResult);

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public MainPageViewModel(INavigationService navigationService,
            IMediaService mediaService,
            IPictureRepository pictureRepository,
            IPageDialogService pageDialogService) 
            : base (navigationService)
        {
            Title = "Detect emotion";
            _mediaService = mediaService;
            _pictureRepository = pictureRepository;
            _pageDialogService = pageDialogService;
        }

        private async void ExecuteTakePhotoCommand()
        {
            try
            {
                var buffer = await _mediaService.TakePhotoAsync();
                if (buffer == null)
                {
                    LatestPicture = null;
                    return;
                }

                EmotionResult = null;
                IsLoading = true;
                try
                {
                    LatestPicture = ImageSource.FromStream(() => new MemoryStream(buffer));
                    var uploadResult = await _pictureRepository.UploadAsync(buffer);
                    if (string.IsNullOrEmpty(uploadResult.Emotion))
                    {
                        await _pageDialogService.DisplayAlertAsync("Info", "Face is not found.", "OK");
                        return;
                    }

                    EmotionResult = $"{uploadResult.Emotion}";
                }
                finally
                {
                    IsLoading = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await _pageDialogService.DisplayAlertAsync("Info", "Upload failed.", "OK");
            }
        }
    }
}
