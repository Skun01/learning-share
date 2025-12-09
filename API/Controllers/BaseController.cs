using Application.Common;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace API.Controllers;

public class BaseController : ControllerBase
{
    protected async Task<ApiResponse<T>> HandleException<T>(Task<T> task)
    {
        try
        {
            var data = await task;
            return ApiResponse<T>.SuccessResponse(data);
        }
        catch(ApplicationException ex)
        {
            return ApiResponse<T>.FailResponse(ex.Message, 200);
        }
        catch(UnauthorizedAccessException ex)
        {
            return ApiResponse<T>.FailResponse(ex.Message, 401);
        }
        catch(KeyNotFoundException ex)
        {
            return ApiResponse<T>.FailResponse(ex.Message, 404);
        }
        catch (Exception ex)
        {
            Log.Logger.Error($"Failed: {ex.Message}\n{ex.StackTrace}");
            return ApiResponse<T>.FailResponse(MessageConstant.CommonMessage.INTERNAL_SERVER_ERROR, 500);
        }
    }
}
