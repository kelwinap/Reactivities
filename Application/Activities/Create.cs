using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace Application.Activities
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Activity Activity { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAcessor;

            public Handler(DataContext context, IUserAccessor userAcessor)
            {
                _context = context;
                _userAcessor = userAcessor;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {

                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAcessor.GetUserName());

                var attendee = new ActivityAttendee
                {
                    AppUser = user,
                    Activity = request.Activity,
                    IsHost = true
                };

                request.Activity.Attendees.Add(attendee);

                await _context.Activities.AddAsync(request.Activity);
                var result = haveChangedDatabase(await _context.SaveChangesAsync());

                if (!result) return Result<Unit>.Failure("Failed to create activity");

                return Result<Unit>.Sucess(Unit.Value);
            }

            private bool haveChangedDatabase(int result)
            {
                return result > 0;
            }
        }
    }
}