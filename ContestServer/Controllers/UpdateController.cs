﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Contest.Shared;
using Contest.Shared.Enums;
using Contest.Shared.Http;
using Contest.Shared.Models;
using ContestServer.Services;
using Microsoft.AspNetCore.Mvc;


namespace ContestServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpdateController : ControllerBase
    {
        private readonly IContestantService contestantService;
        private readonly ITimeService timeService;
        private readonly IGameService gameService;
        public const int UpdateRateLimitInSeconds = 1;

        public UpdateController(IContestantService contestantService, ITimeService timeService, IGameService gameService)
        {
            this.contestantService = contestantService ?? throw new ArgumentNullException(nameof(contestantService));
            this.timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            this.gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        [HttpPost]
        public UpdateResponse Post([FromBody]UpdateRequest request)
        {
            var response = new UpdateResponse();
            if (request == null)
            {
                response.IsError = true;
                response.ErrorMessage = "Invalid request";
                return response;
            }

            if(!contestantService.ContestantExists(request.Token))
            {
                response.IsError = true;
                response.ErrorMessage = "You are not a registered player.  Register to play at /register";
                return response;
            }

            var contestant = contestantService.GetContestantByToken(request.Token);

            if(contestant == null)
            {
                response.IsError = true;
                response.ErrorMessage = "You are not a registered player.  Register to play at /register";
                return response;
            }

            var timeSinceLastUpdate = timeService.Now() - contestant.LastSeen;
            if(timeSinceLastUpdate.TotalSeconds < UpdateRateLimitInSeconds)
            {
                response.IsError = true;
                response.ErrorMessage = $"The rate limit requires you call this endpoint no more than once every {UpdateRateLimitInSeconds} seconds.";
                return response;
            }

            // at this point we are done with validation
            contestant = new Contestant
            (
                contestant.Name,
                contestant.Token,
                contestant.LastSeen,
                request.GenerationsComputed,
                contestant.StartedGameAt,
                contestant.EndedGameAt,
                request.ResultBoard
            );

            var gameStatus = gameService.GetGameStatus();

            response.GameState = gameStatus.IsStarted ? GameState.InProgress : GameState.NotStarted;
            if (gameStatus.IsGameOver)
                response.GameState = GameState.Over;

            if(gameStatus.IsStarted)
            {
                contestant = StartContestantIfNotStarted(contestant);
                response.SeedBoard = gameStatus.SeedBoard;
                response.GenerationsToCompute = gameStatus.NumberGenerations;
            }

            response.IsError = false;

            contestantService.UpdateContestant(contestant);
            return response;
        }

        private Contestant StartContestantIfNotStarted(Contestant contestant)
        {
            if(contestant.StartedGameAt == null)
            {
                return contestant.UpdateStartedGameAt(timeService.Now());
            }
            return contestant;
        }
    }
}
