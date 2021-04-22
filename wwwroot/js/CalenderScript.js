
let modalInstance;

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
    modalInstance.open();
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


//let crontabs = document.querySelectorAll(".cron-tabs")
//let crontabsInstance;
//crontabsInstance = M.Tabs.init(crontabs, {});
//Calender Modal
$(() => {

    $(".cron-tabs").tabs()
    
   

    //crontabsInstance?.select("minutes-tab")

    cronSelect = M.FormSelect.init(document.querySelectorAll('.cron-select'))

    //$('.cron-tabs').tabs();
    //$("#ScheduleTaskForm").find("select").formSelect();
    //$("#ScheduleTaskForm").find("select").css("display","block")
    let modal = document.querySelector("#calendar-modal");
    let options = {
        preventScrolling: false,
        onOpenEnd : $('.cron-tabs').tabs("select", "minutely")
    }
    modalInstance = M.Modal.init(modal, options)

    console.log(modalInstance)
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
        FormData.activeSchedule = $("#cron-schedule-tabs [role=tab].active").attr("href")

        
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
                        "startat": `${$("#hourly-every-start-at").val().toString().trim()}:${$("#hourly-every-start-at-2").val().toString().trim()}`
                    }
                }

                FormData = {
                    ...FormData,
                    ScheduleRecurrence: "Hourly",
                    ScheduleData: {...temp}
                }
                console.log(FormData)
                break;
            case "#daily":
                var temp = {
                    recurance: $("[name=cronDaily]:checked").val().toString().trim(),
                    startat: `${$("#cron-dailyHour-startat").val().trim()}:${$("#cron-dailyMinutes-startat").val().trim()}`
                }
                FormData = {
                    ...FormData,
                    ScheduleRecurrence: "Daily",
                    ScheduleData: { ...temp }
                }

                break;
            default:
        }

        console.dir(FormData)
        
        $.ajax({
            url: "/api/v1.1/Scheduler/SimpleTask",
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
