using DevConsultHack.Client.Models;
using DevConsultHack.Client.Repositories;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DevConsultHack.Client.ViewModels
{
	public class HistoryPageViewModel : ViewModelBase
	{
        private readonly IPictureRepository _pictureRepository;
        private readonly IPageDialogService _pageDialogService;
        private string _nextToken;
        public string NextToken
        {
            get { return _nextToken; }
            set { SetProperty(ref _nextToken, value); }
        }

        private ObservableCollection<Transaction> TransactionsSource { get; } = new ObservableCollection<Transaction>();

        public ReadOnlyObservableCollection<Transaction> Transactions { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private DelegateCommand _loadMoreHistoriesCommand;
        public DelegateCommand LoadMoreHistoriesCommand =>
            _loadMoreHistoriesCommand ?? (_loadMoreHistoriesCommand = new DelegateCommand(ExecuteLoadMoreHistoriesCommand, CanExecuteLoadMoreHistoriesCommand));

        public HistoryPageViewModel(INavigationService navigationService, 
            IPictureRepository pictureRepository,
            IPageDialogService pageDialogService) 
            : base(navigationService)
        {
            Title = "Histories";
            _pictureRepository = pictureRepository;
            _pageDialogService = pageDialogService;
            Transactions = new ReadOnlyObservableCollection<Transaction>(TransactionsSource);
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await LoadHistoriesAsync();
        }

        private async Task LoadHistoriesAsync(bool isIncrementalLoading = false)
        {
            try
            {
                IsLoading = true;
                if (isIncrementalLoading && string.IsNullOrEmpty(NextToken))
                {
                    return;
                }

                var histories = await _pictureRepository.GetHistoriesAsync(NextToken);
                if (histories == null)
                {
                    return;
                }

                NextToken = histories.NextToken;

                foreach (var tx in histories.Transactions)
                {
                    TransactionsSource.Add(tx);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await _pageDialogService.DisplayAlertAsync("Info", "An error occured.", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void ExecuteLoadMoreHistoriesCommand() => await LoadHistoriesAsync(true);

        private bool CanExecuteLoadMoreHistoriesCommand() => !string.IsNullOrEmpty(NextToken);


    }
}
