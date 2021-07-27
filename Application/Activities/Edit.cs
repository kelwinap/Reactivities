using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistance;

namespace Application.Activities
{
    public class Edit
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
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.Activity.Id);

                if (activity == null) return null;

                _mapper.Map(request.Activity, activity);

                var result = haveChangedDatabase(await _context.SaveChangesAsync());

                if (!result) return Result<Unit>.Failure("Failed to edit activity");

                return Result<Unit>.Sucess(Unit.Value);
            }

            private bool haveChangedDatabase(int result)
            {
                return result > 0;
            }
        }
    }
}