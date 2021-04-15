

let nextRefreshEle = $("#next-refresh-time");

let Endpoints = {
    getTasks: "/api/v1/Scheduler",
    PauseTasks: "/api/v1/Scheduler/Pause/",
    UpauseTasks: "/api/v1/Scheduler/Resume",
    ScheduleCronTask: "/api/v1/Scheduler/Task",
    ScheduleSimpleTask: "/api/v1.1/Scheduler/SimpleTask"
}




let taskTable = $("#task-dataTable").DataTable({
    dom: "<'row '<'col s12 m3'l><'col s12 m5 center'B><'col s12 m4'f>>" +
        "<'col s12't>" +
        "<'row center'<'col s12 m12'p>>",
    buttons: [
        { //NEW BTN
            text: "New",
            action: function (e, dt, node, config) {
                $("#calendar-tab").trigger("click");
                setTimeout(() => {
                    $('#calendar-modal').iziModal('open');;
                }, 200);
                
            },
            className: "blue"
        },
        { //RELOAD BTN
            text: 'Reload',
            action: function (e, dt, node, config) {
                dt.ajax.reload();
            },
            className: "green"
        },
        { //PAUSE BTN
            text: "Pause",
            action: function (e, dt, node, config) {
                let rowCount = dt.rows({ selected: true }).data().length;
                if (rowCount > 0) {
                    let msgText = 'Are you sure you want to <strong>Pause</strong> the Task';

                    if (rowCount > 1) msgText = msgText + 's';
                    var rows = []
                    for (var i = 0; i <= rowCount - 1; i++) {
                        rows.push(dt.rows({ selected: true }).data()[i].DT_RowId)
                    }
                    swal({
                        text: msgText,
                        icon: 'warning',
                        dangerMode: true,
                        buttons: {
                            cancel: "Cancel",
                            confirm: {
                                text: "Yes, Pause",
                                value: "confirm",
                                className: "myClass"
                            }
                        }
                    }).then(btnVal => {
                        switch (btnVal) {
                            case "confirm":


                                let pauseData = {
                                    TaskIds: rows,
                                    count: rowCount
                                }
                                console.log(JSON.stringify(pauseData))
                                $.ajax({
                                    url: Endpoints.PauseTasks,
                                    contentType: "application/json",
                                    method: "PUT",
                                    data: JSON.stringify(pauseData),
                                    success: function (result) {

                                        swal("Pause", temp.reponse, "success")
                                    },
                                    error: function (jqXHR, exception) {
                                        swal("Pause", jqXHR.statusText, "error")
                                    },


                                }).fail((jqXHR, expetion) => {
                                    swal("Pause", "somthing went wrong", "error")
                                })


                                break;
                            case null:
                                swal.stopLoading();
                                swal.close();
                                break;
                            default:
                                swal("Pause", "No Action", "info")

                        }
                    }).catch(err => {
                        if (err) {
                            swal("Oh noes!", err, "error");
                        } else {
                            swal.stopLoading();
                            swal.close();
                        }
                    });

                } else {
                    swal("Opps", "It appears nothin was selected from the table below", "info")
                }
            },
            className:"btn-secondary"
        },
        { //UNPAUSE BTN
            text: "Resume",
            action: function (e, dt, node, config) {
                let rowCount = dt.rows({ selected: true }).data().length;
                if (rowCount > 0) {
                    let msgText = 'Are you sure you want to Unpause the Task';

                    if (rowCount > 1) msgText = msgText + 's';
                    var rows = []
                    for (var i = 0; i <= rowCount - 1; i++) {
                        rows.push(dt.rows({ selected: true }).data()[i].DT_RowId)
                    }
                    swal({
                        text: msgText,
                        icon: 'warning',
                        dangerMode: true,
                        buttons: {
                            cancel: "Cancel",
                            confirm: {
                                text: "Yes, Unpause",
                                value: "confirm",
                                className: "myClass"
                            }
                        }
                    }).then(btnVal => {
                        switch (btnVal) {
                            case "confirm":
                                

                                let unpauseData = {
                                    TaskIds: rows,
                                    count: rowCount
                                }
                                console.log(JSON.stringify(unpauseData))
                                $.ajax({
                                    url: Endpoints.UpauseTasks,
                                    contentType: "application/json",
                                    method: "PUT",
                                    data: JSON.stringify(unpauseData),
                                    success: function (result) {
                                         
                                        swal("Unpause", temp.reponse, "success")
                                    },
                                    error: function (jqXHR, exception) {
                                        swal("Unpause", jqXHR.statusText, "error")
                                    },


                                }).fail((jqXHR, expetion) => {
                                    swal("Unpause", "somthing went wrong", "error")
                                })


                                break;
                            case null:
                                swal.stopLoading();
                                swal.close();
                                break;
                            default:
                                swal("Unpause", "No Action", "info")

                        }
                    }).catch(err => {
                        if (err) {
                            swal("Oh noes!", err, "error");
                        } else {
                            swal.stopLoading();
                            swal.close();
                        }
                    });

                } else {
                    swal("Opps", "It appears nothin was selected from the table below", "info")
                }
            },
            className: "btn-info"
        },
        { //DELETE BTN
            text: 'Delete',
            action: function (e, dt, node, config) {

                let rowCount = dt.rows({ selected: true }).data().length;
                if (rowCount > 0) {
                    let msgText = 'Are you sure you want to Delete the Task';

                    if (rowCount > 1) msgText = msgText + 's';
                    var rows = []
                    for (var i = 0; i <= rowCount - 1; i++) {
                        rows.push(dt.rows({ selected: true }).data()[i].DT_RowId)
                    }
                    swal({
                        text: msgText,
                        icon: 'warning',
                        dangerMode:true,
                        buttons: {
                            cancel: "Cancel",
                            confirm: {
                                text: "Yes, Delete",
                                value: "confirm",
                                className:"myClass"
                            }
                        }
                    }).then(btnVal => {
                        switch (btnVal) {
                            case "confirm":
                                let deleteUrl = "/api/v1/Scheduler/DeleteTasks"
                                
                                let deleteData = {
                                    TaskIds: rows,
                                    count:rowCount
                                }
                                console.log(JSON.stringify(deleteData))
                                $.ajax({
                                    url: deleteUrl,
                                    contentType: "application/json",
                                    method: "DELETE",
                                    data: JSON.stringify(deleteData),
                                    success: function (result) {
                                        let temp = JSON.parse(result)
                                        swal("Delete", temp.response, "success")
                                    },
                                    error: function (jqXHR, exception) {
                                        swal("Delete", jqXHR.statusText , "error")
                                    },
                                    
                                    
                                }).fail((jqXHR, expetion) => {
                                    swal("Delete", "somthing went wrong", "error")
                                })

                                
                                break;
                            case null:
                                swal.stopLoading();
                                swal.close();
                                break;
                            default:
                                swal("Delete", "No Action", "info")
                               
                        }
                    }).catch(err => {
                        if (err) {
                            swal("Oh noes!", "The AJAX request failed!", "error");
                        } else {
                            swal.stopLoading();
                            swal.close();
                        }
                    });

                } else {
                    swal("Opps", "It appears nothin was selected from the table below","info")
                }
                console.log(rowCount)
                               
            },
            className:"red"

        }
    ],
    select: 'multi',
    "ajax": {
        url: "api/v1/Scheduler",
        dataSrc: ""
    },
    "columns": [
        { "data": "TaskName" },
        { "data": "Group" },
        { "data": "PreviousFireTime" },
        { "data": "NextFireTime" },
        { "data": "Discription" }
    ],
});

//taskTable.buttons().container()
//    .appendTo($('#task-dataTable_length', taskTable.table().container()));

const InitElements = () => {
    nextRefreshEle.text("The Table is Refreshed every 6 sec")
}

//Task Table Init


//On Page Load
$(() => {
    InitElements()
    $('.tabs').tabs();
    //$('label select').formSelect({ classes: 'row col s10' });
})




//Task Data Table


//taskTable Refresh Data
setInterval(function () {
    taskTable.ajax.reload();
}, 6000)
