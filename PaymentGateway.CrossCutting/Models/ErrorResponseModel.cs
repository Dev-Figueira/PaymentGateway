﻿namespace PaymentGateway.CrossCutting.Models
{
    public class ErrorResponseModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
