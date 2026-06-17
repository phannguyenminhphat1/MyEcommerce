using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace MyEcommerce.Admin.Roles
{
    public class CreateUpdateRoleDtoValidator : AbstractValidator<CreateUpdateRoleDto>
    {
        public CreateUpdateRoleDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
