using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Persistance;

namespace Application.Activities
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {

            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.Id);

                if (activity == null) return null;

                _context.Remove(activity);

                var result = haveChangedDatabase(await _context.SaveChangesAsync());

                if (!result) return Result<Unit>.Failure("Failed to delete the activity");

                return Result<Unit>.Sucess(Unit.Value);
            }

            private bool haveChangedDatabase(int result)
            {
                return result > 0;
            }
        }
    }
}