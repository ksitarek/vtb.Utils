using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vtb.Utils
{
    public abstract class SafeControllerBase : ControllerBase
    {
        protected readonly IDictionary<Type, Func<IActionResult>> _exceptionToResponseMap = new Dictionary<Type, Func<IActionResult>>();
        private readonly ILogger<SafeControllerBase> _logger;

        protected SafeControllerBase(ILogger<SafeControllerBase> logger)
        {
            _logger = logger;
        }

        protected async Task<IActionResult> SafeInvoke(Func<Task<IActionResult>> func, Dictionary<Type, Func<IActionResult>> overrides = null)
        {
            try
            {
                return await func.Invoke();
            }
            catch (Exception e)
            {
                var exceptionType = e.GetType();

                var map = _exceptionToResponseMap;
                if (overrides != null)
                    map = MergeMaps(map, overrides);

                if (!map.ContainsKey(exceptionType))
                {
                    _logger.LogError(e, $"UNHANDLED: {e.Message}");
                    return StatusCode(500);
                }

                _logger.LogWarning(e, e.Message);

                var mapping = map[exceptionType];
                return mapping.Invoke();
            }
        }

        private Dictionary<Type, Func<IActionResult>> MergeMaps(
            IDictionary<Type, Func<IActionResult>> left,
            IDictionary<Type, Func<IActionResult>> right)
        {
            var result = left.ToDictionary(x => x.Key, x => right.ContainsKey(x.Key) ? right[x.Key] : x.Value);

            foreach (var (key, value) in right.Where(x => !result.ContainsKey(x.Key)))
                result.Add(key, value);

            return result;
        }
    }
}