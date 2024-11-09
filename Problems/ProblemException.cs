using System.Text.Json;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Problems;

[Serializable]
public class ProblemException(string error, string message) : Exception(message)
{
    public string Error { get; set; } = error;
    public string Message { get; set; } = message;
}