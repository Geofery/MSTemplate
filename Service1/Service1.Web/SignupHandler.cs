﻿using System;
using NServiceBus;
using Application.Commands;
using Application.Events;
using Domain.Models;
using Domain.Repositories;
using SharedMessages;

namespace Web
{
	public class SignupHandler : IHandleMessages<SignupCommand>
	{
        private readonly IUserRepository _userRepository;

        public SignupHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task Handle(SignupCommand message, IMessageHandlerContext context)
        {
            var signup = new User(new Guid(), message.Name, message.Email, message.Password);
          
            var result = await _userRepository.SaveMemberAsync(signup).ConfigureAwait(false);
 

            var signupCompleted = new SharedMessages.SignupCompleted(result.LeadId, result.Name, result.Email, result.Password);
            Console.WriteLine($"MESSAGE IN HANDLER {message}");
            await context.Publish(signupCompleted).ConfigureAwait(false);
        }
    }
}

