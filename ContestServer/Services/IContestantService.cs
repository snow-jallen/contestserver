﻿using Contest.Shared;
using Contest.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContestServer.Services
{
    public interface IContestantService
    {
        void AddContestant(Contestant contestant);
        IEnumerable<Contestant> GetContestants();
        Contestant GetContestantByToken(string token);
        void RemoveContestant(Contestant contestant);
        void UpdateContestant(Contestant contestant);
        bool ContestantExists(string token);
        void ResetContestantList();
    }
}
