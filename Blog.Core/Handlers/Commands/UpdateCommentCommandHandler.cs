﻿using Blog.Contracts.Data;
using Blog.Contracts.Data.Entities;
using Blog.Contracts.DTO;
using Blog.Core.Exceptions;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Handlers.Commands
{
    public class UpdateCommentCommand : IRequest<int>
    {
        public int Id { get; set; }
        public UpdateCommentDTO Model { get; }
        public UpdateCommentCommand(UpdateCommentDTO model, int id)
        {
            this.Model = model;
            this.Id = id;
        }
    }

    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, int>
    {
        private readonly IUnitOfWork _repository;
        private readonly IValidator<UpdateCommentDTO> _validator;

        public UpdateCommentCommandHandler(IUnitOfWork repository, IValidator<UpdateCommentDTO> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<int> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            UpdateCommentDTO model = request.Model;

            var result = _validator.Validate(model);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                throw new InvalidRequestBodyException
                {
                    Errors = errors
                };
            }

            var entityToUpdate = _repository.Comments.Get(request.Id);
            if (entityToUpdate == null)
            {
                return default;
            }
            else
            {
                entityToUpdate.Content = model.Content;
                entityToUpdate.FirstName = model.FirstName;
                entityToUpdate.LastName = model.LastName;
                entityToUpdate.Email = model.Email;
                entityToUpdate.AddedOn = model.AddedOn;
                _repository.Comments.Update(entityToUpdate);
                await _repository.CommitAsync();

                return entityToUpdate.Id;
            }
            //var entity = new Comment
            //{
            //    Content = model.Content,
            //    FirstName = model.FirstName,
            //    LastName = model.LastName,
            //    Email = model.Email,
            //    PostId = model.PostId,

        }
    }
}