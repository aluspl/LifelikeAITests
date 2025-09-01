using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.StepCache.Models;

namespace AiAgent.Api.Domain.StepCache.Queries;

public class GetAllStepCachesQuery : IQuery<ICollection<StepCacheResponse>> { }