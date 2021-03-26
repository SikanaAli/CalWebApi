

let nextRefreshEle = $("#next-refresh-time");



let taskTable = $("#task-dataTable").DataTable({
    dom: '<<"topElements"B>fltp>',
    buttons: [
        {
            text: 'Reload',
            action: function (e, dt, node, config) {
                dt.ajax.reload();
            }
        }
    ],
    select: 'single',
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
