
function selectDate(date) {
    $('#calendar').updateCalendarOptions({
        date: date
    });
    console.log("Single Click");
}

function onDoubleClick(date) {
    $('#calendar').updateCalendarOptions({
        date: date
    });
    $('#calendar-modal').iziModal('open');
    $("#cron-expression-desc").text(cronstrue.toString($("#cron-output").val()))
    console.log("Doublick done");
}

var defaultConfig = {
    weekDayLength: 1,
    date: new Date(),
    onClickDate: selectDate,
    onDoubleClickDate: onDoubleClick,
    showYearDropdown: true,
};

$('#calendar').calendar(defaultConfig);
$('.day').addClass('noselect');


//Calender Modal
$("#calendar-modal").iziModal({
    title: "<strong>Schedule New Task</strong>",
    padding: "2rem",
    radius: 5,
    width: "60%",
    fullscreen: true,
    theme: "light",

    left: 0,
    right: 0,

})


//Form Validattion

jQuery.validator.setDefaults({
    debug: true,
    success: "valid"
});

jQuery.validator.addMethod("isValidCron", function (value, element) {
    return this.optional(element) || /^http:\/\/mycorporatedomain.com/.test(value);
}, "Please specify the correct domain for your documents");

var Validator = $("#ScheduleTaskForm").validate({
    rules: {
        "cron-task-name": {
            required: true,
            minlength: 4
        },
        "cron-output": {
            required: true,
            
        },
        "cron-callback": {
            required: true
        }
    },
    submitHandler: (from) => {

        //FetchData from Form
        var FormData = {
            "taskName": $("#cron-task-name").val().trim().toString(),
            "dateCreated": new Date().toISOString(),
            "callBack": $("#cron-callback").val().trim().toString(),
            "ScheduleCallbackType": $("#cron-callback-type").val().trim().toString(),
            "Description": $("#cron-description").val().trim().toString()
        }

        //Get Active Schedule
        FormData.activeSchedule = $("#cron-schedule-tabs [data-toggle=tab].active").attr("href")

        switch (FormData?.activeSchedule) {
            case "#minutely":

                FormData = {
                    ...FormData,
                    ScheduleRecurrence: "Minutes",
                    ScheduleData: {
                        every: $("#cron-every-minutely").val().trim().toString()
                    }
                }
                console.log(FormData);
                break;
            case "#hourly":

                var temp = {}
                if ($("[name=hourly-every]:checked").val().toString() === "option1") {
                    temp = {
                        "every": $("#hourly-every-interval").val().toString().trim()
                    }
                } else if ($("[name=hourly-every]:checked").val().toString() === "option2") {
                    temp = {
                        "startat": `${$("#hourly-every-interval").val().toString().trim()}:${$("#hourly-every-start-at-2").val().toString().trim()}`
                    }
                }

                FormData = {
                    ...FormData,
                    ScheduleRecurrence: "Hourly",
                    ScheduleData: {...temp}
                }
                console.log(FormData)
                break;
            default:
        }
        
        $.ajax({
            url: "/api/v1.1/Scheduler/Task",
            method: "POST",
            contentType:"application/json",
            data: JSON.stringify(FormData),
            success: (result) => {
                swal("Task Creation", JSON.stringify(result), "success")
                console.log(result)
            },
            error: (result) => {
                swal("Task Creation", result.responseText, "error")
                console.log(result)
            },
            
        }).fail(result => {
               console.log(result) 
        });
    }
})
