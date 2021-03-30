

let nextRefreshEle = $("#next-refresh-time");



let taskTable = $("#task-dataTable").DataTable({
    dom: "<'row'<'col-sm-12 col-md-6'Bl><'col-sm-12 col-md-6'f>>" +
        "<'row'<'col-sm-12't>>" +
        "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
    buttons: [
        {
            text: "New",
            action: function (e, dt, node, config) {
                $("#calendar-tab").trigger("click");
            }
        },
        {
            text: 'Reload',
            action: function (e, dt, node, config) {
                dt.ajax.reload();
            }
        }, {
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
                                let deleteUrl = "/api/v1/Scheduler/DeleteTaskss"
                                
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
                                        swal("Delete", result.response, "success")
                                    },
                                    error: function (result) {
                                        swal("Delete", result.response, "error")
                                    }
                                    
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
                

               
            }
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
    ],
});

taskTable.buttons().container()
    .appendTo($('#task-dataTable_length', taskTable.table().container()));

const InitElements = () => {
    nextRefreshEle.text("The Table is Refreshed every 6 sec")
}

//Task Table Init


//On Page Load
$(() => {
    InitElements()
})




//Task Data Table


//taskTable Refresh Data
setInterval(function () {
    taskTable.ajax.reload();
}, 6000)
