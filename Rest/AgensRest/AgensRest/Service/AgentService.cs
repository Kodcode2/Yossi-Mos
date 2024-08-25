﻿using AgensRest.Models;
using AgensRest.Dto;
using AgentsApi.Data;

using Microsoft.EntityFrameworkCore;
using static AgensRest.Service.AgentService;
using Microsoft.AspNetCore.Mvc;

namespace AgensRest.Service
{
    public class AgentService(IServiceProvider serviceProvider, ApplicationDBContext context) : IAgentService
    {
        private IMissionService missionService = serviceProvider.GetRequiredService<IMissionService>();
        private ITargetService targetService = serviceProvider.GetRequiredService<ITargetService>();

        private readonly Dictionary<string, (int, int)> Direction = new()
        {
            {"n", (0, 1)},
            {"s", (0, -1)},
            {"e", (-1, 0)},
            {"w", (1, 0)},
            {"ne", (-1, 1)},
            {"nw", (1, 1)},
            {"se", (-1, -1)},
            {"sw", (1, -1)}
        };
        public async Task<List<AgentModel>> GetAgentsAsync()
        {
            var a = context.Agents;
            return await context.Agents.ToListAsync();
        }


        public async Task<ActionResult<AgentModel>> UpdateAgentAsync(int id, AgentModel agentModel)
        {
            if (!context.Agents.Any(a => a.Id == id))
            {
                return null;
            }
            try
            {
                var agent = await context.Agents.FirstOrDefaultAsync(a => a.Id == id);
                agent!.Image = agentModel.Image;
                agent.Nickname = agentModel.Nickname;
                await context.SaveChangesAsync();
                return agent;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<ActionResult<AgentModel>> DeleteAgentModelAsync(int id)
        {
            var agentModel = await context.Agents.FindAsync(id);
            if (agentModel == null)
            {
                return null;
            }

            context.Agents.Remove(agentModel);
            await context.SaveChangesAsync();

            return agentModel;
        }


    
        public async Task<AgentModel> Pin(PinDto pin, int id)
        {
            AgentModel? agent = await context.Agents.FirstOrDefaultAsync(t => t.Id == id);
            if (agent == null)
            {
                throw new Exception("Target not found");
            }
            agent.X = pin.X;
            agent.Y = pin.Y;
            await context.SaveChangesAsync();
            return agent;
        }


        public async Task<IdDto> CreateAgentModel(AgentDto agentDto)
        {
            try
            {
                AgentModel agent = new()
                {
                    Image = agentDto.PhotoUrl,
                    Nickname = agentDto.Nickname,
                };
                await context.Agents.AddAsync(agent);
                await context.SaveChangesAsync();
                var a = await context.Agents
                    .FirstOrDefaultAsync(A => A.Image == agent.Image && A.Nickname == agent.Nickname);
                IdDto idDto = new() { Id = a.Id };
                return idDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<AgentModel> FindAgentById(int id)
        {
            AgentModel? agent = await context.Agents
                .FirstOrDefaultAsync(t => t.Id == id);
            if (agent == null)
            {
                throw new Exception("Target not found");
            }
            return agent;
        }

    }
}

