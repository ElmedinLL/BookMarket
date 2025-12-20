var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/company/getall' },
 
        "columns": [
        { data: 'name', "width":"25%" },
            { data: 'streetAddress', "width": "15%" },
            { data: 'city', "width": "10%" },
            { data: 'state', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-100 btn-group" role="group">
                    <a href="/Admin/Company/Upsert?id=${data}" class="btn btn-primary mx-2">
                    <i class="bi bi-pencil-square"></i>Edit</a>
                    <a onClick=Delete('/Admin/Company/Delete/${data}') class="btn btn-danger mx-2">
                    <i class="bi bi-trash-fill"></i>Delete</a>
                    </div>`
                },
                "width": "25%"
            }
        ]
        });
}

function Delete(url) {
  
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d63030",
        cancelButtonColor: "#79797d",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }

            })
        }
    });
}


