using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.BranchVMs;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;

namespace BaSalesManagementApp.MVC.Validators.BranchValidators
{
    public class BranchCreateValidation : AbstractValidator<BranchCreateVM>
    {


        public BranchCreateValidation()
        {


            RuleFor(s => s.Name)
                 .NotEmpty()
                 .WithMessage(Messages.BRANCH_NAME_NOT_EMPTY)
                 .MaximumLength(128)
                 .WithMessage(Messages.BRANCH_NAME_MAXIMUM_LENGTH)
                 .MinimumLength(2)
                 .WithMessage(Messages.BRANCH_NAME_MINIMUM_LENGTH);

            RuleFor(s => s.Address)
                .NotEmpty()
                .WithMessage(Messages.BRANCH_ADDRESS_NOT_EMPTY)
                .MaximumLength(128)
                .WithMessage(Messages.BRANCH_ADDRESS_MAXIMUM_LENGTH)
                .MinimumLength(2)
                .WithMessage(Messages.BRANCH_ADDRESS_MINIMUM_LENGTH);

            RuleFor(s => s.CompanyId)
                .NotEmpty()
                .WithMessage(Messages.BRANCH_COMPANY_RELATION);
        }
    }
}
