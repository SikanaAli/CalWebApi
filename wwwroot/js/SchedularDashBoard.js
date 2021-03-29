

let nextRefreshEle = $("#next-refresh-time");



let taskTable = $("#task-dataTable").DataTable({
    dom: "<'row'<'col-sm-12 col-md-6'Bl><'col-sm-12 col-md-6'f>>" +
        "<'row'<'col-sm-12'tr>>" +
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
                var tempRows = dt.rows({ selected: true }).data()
                console.log(rowCount)
                var rows = {}
                //for (var i = 0; i < rowCount - 1; i++) {
                //    temp = tempRows[i].DT_RowId
                //    rows = { ...rows, temp}
                //}

                console.log(tempRows[0].DT_RowId)
                //swal("Here's a message!", "It's pretty, isn't it?")
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
