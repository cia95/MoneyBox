using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            var account = this.accountRepository.GetAccountById(fromAccountId);

            var accountBalance = account.Balance - amount;
            if (accountBalance < 0m)
            {
                this.notificationService.NotifyInsufficientFunds(account.User.Email);
            }

            if (accountBalance < 500m)
            {
                this.notificationService.NotifyFundsLow(account.User.Email);
            }

            account.Balance = accountBalance;
            account.Withdrawn = account.Withdrawn - amount;

            this.accountRepository.Update(account);

            this.notificationService.NotifySuccessfulTransaction(account.User.Email);
        }
    }
}
