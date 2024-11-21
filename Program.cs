using staffschedulerlibrary.Models;
using DatabaseLogic;


var builder = WebApplication.CreateBuilder(args);

// NEED THIS (to allow all operations for client to make request to cloud services like firebase, a ruleset for application to talk to each other)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .SetIsOriginAllowed(origin => true);// allow any origin
            //.AllowCredentials();
        });
});

// Add services to the container.

var app = builder.Build();

FirestoreManager _databaselogic = new FirestoreManager();
_databaselogic.InitFireStore();

//routing codes
//test code
app.MapGet("/", () => "Connection successful!");

//map to endpoint for data saving 


// POST endpoint for adding staff
app.MapPost("/Add-Staffs", async (Staff staff) =>
{
    try
    {
        await _databaselogic.SaveStaffAsync(staff); // Save staff to Firestore
        return Results.Created($"/Get-Staff/{staff.Id}", staff); // Return created resource
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error saving staff: {ex.Message}");
    }
});

// GET endpoint for retrieving all staff
app.MapGet("/Get-Staffs", async () =>
{

    var staffList = await _databaselogic.GetAllStaffAsync(); // Retrieve all staff from Firestore
    return Results.Ok(staffList); // Return the list of staff members
});

// POST endpoint for adding shifts
app.MapPost("/Add-Shifts", async (Shift shift) =>
{
    if (shift == null)
    {
        return Results.BadRequest("Shift cannot be null.");
    }

    await _databaselogic.SaveShiftAsync(shift); // Save shift to Firestore
    return Results.Created($"/Get-Shift/{shift.ShiftId}", shift); // Return created resource
});


// GET endpoint for retrieving shift data by shiftId
app.MapGet("/Get-Shift", async () =>
{
    var shiftList = await _databaselogic.GetAllShiftAsync(); // Retrieve all shifts from Firestore
    return Results.Ok(shiftList); // Return the list of shifts
});

// Map POST endpoint for adding a TaskAllocation
app.MapPost("/task-allocations", async (TaskAllocation taskAllocation) =>
{
    if (taskAllocation == null)
    {
        return Results.BadRequest("TaskAllocation cannot be null.");
    }

    await _databaselogic.SaveTaskAsync(taskAllocation); // Save task allocation to Firestore
    return Results.Created($"/api/taskallocations/{taskAllocation.TaskId}", taskAllocation); // Return created resource
});

// Map GET endpoint for retrieving all TaskAllocations
app.MapGet("/Get-task-allocations", async () =>
{
    var taskAllocations = await _databaselogic.GetAllTasksAsync(); // Retrieve all task allocations from Firestore
    return Results.Ok(taskAllocations); // Return the list of task allocations
});

app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
// NEED THIS


app.Run();
