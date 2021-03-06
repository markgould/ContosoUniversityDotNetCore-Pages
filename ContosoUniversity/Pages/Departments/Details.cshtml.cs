﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Departments
{
    public class Details : PageModel
    {
        private readonly IMediator _mediator;

        public Model Data { get; private set; }

        public Details(IMediator mediator) => _mediator = mediator;

        public async Task OnGetAsync(Query query)
            => Data = await _mediator.Send(query);

        public class Query : IRequest<Model>
        {
            public int Id { get; set; }
        }

        public class Model
        {
            public string Name { get; set; }

            public decimal Budget { get; set; }

            public DateTime StartDate { get; set; }

            public int Id { get; set; }

            [Display(Name = "Administrator")]
            public string AdministratorFullName { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Department, Model>();
        }
        
        public class QueryHandler : IRequestHandler<Query, Model>
        {
            private readonly SchoolContext _context;

            public QueryHandler(SchoolContext context) => _context = context;

            public Task<Model> Handle(Query message, CancellationToken token) => _context.Departments
                .FromSql(@"SELECT * FROM Department WHERE DepartmentID = {0}", message.Id)
                .ProjectTo<Model>()
                .SingleOrDefaultAsync(token);
        }
    }
}