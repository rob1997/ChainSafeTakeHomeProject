using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;


public enum ResponseStatus
{
    Success,
    Error
}

public class Response<T> where T : IDataModel
{
    public bool Done { get; private set; }

    public ResponseStatus Status { get; private set; }

    public T Result { get; private set; }
    
    public string Message { get; private set; }

    public Response()
    {
        
    }
    
    public Response(string errorMessage)
    {
        Done = true;

        Status = ResponseStatus.Error;

        Result = default;
        
        Message = errorMessage;
    }
    
    public static Response<T> FromRawData(string rawData)
    {
        Response<T> response;
        
        try
        {
            T result = JsonConvert.DeserializeObject<T>(rawData);
            
            response = new Response<T>
            {
                Done = true,
                
                Status = ResponseStatus.Success,
                
                Result = result,
                
                Message = "SUCCESS"
            };

            return response;
        }
        
        catch (Exception e)
        {
            response = new Response<T>($"Error Deserializing Response Data, {e.Message}");

            return response;
        }
    }
}
