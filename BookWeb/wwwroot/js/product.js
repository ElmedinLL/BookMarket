var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall' },
 
        "columns": [
        { data: 'title', "width":"25%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "15%" },
            { data: 'category.name', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-100 btn-group" role="group">
                    <a href="/Admin/Product/Upsert?id=${data}" class="btn btn-primary mx-2">
                    <i class="bi bi-pencil-square"></i>Edit</a>
                    <a onClick=Delete('/Admin/Product/Delete/${data}') class="btn btn-danger mx-2">
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


