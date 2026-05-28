var categoryTable;
$(document).ready(function () {
    loadCategoryTable();
});
function loadCategoryTable() {
    if ($.fn.DataTable.isDataTable('#tbCTData')) {
        $('#tbCTData').DataTable().destroy();
    }
    categoryTable = $('#tbCTData').DataTable({
        "ajax": {
            "url": "/Admin/Category/GetAllCategory",
            "type": "GET",
            "datatype": "json"
        },
        "responsive": true,
        "autoWidth": false,
        // IMPORTANT
        "ordering": false,
        "pagingType": "simple_numbers",
        "stripeClasses": [],
        "language": {
            "emptyTable": "No categories found",
            "search": "Search:",
            "lengthMenu": "Show _MENU_ entries",
            "info": "Showing _START_ to _END_ of _TOTAL_ categories",
            "paginate": {
                "previous": "Prev",
                "next": "Next"
            }
        },
        "columns": [
                        // CATEGORY COLUMN

            {
                "data": null,
                "width": "75%",
                "render": function (data) {
                    // PARENT CATEGORY
                    if (data.isParentCategory) {
                        return `
<div class="category-parent">
    <div class="category-icon">
        <i class="fas fa-folder"></i>
    </div>
    <div class="category-parent-title">
        ${data.name}
    </div>
</div>
`;
                    }
                    // CHILD CATEGORY
                    return `
<div class="category-child">
    <div class="category-child-icon">
        <i class="fas fa-level-down-alt"></i>
    </div>
    <div class="category-child-title">
        <span class="child-parent-name">
            ${data.parentCategoryName}
        </span>
        <span class="child-divider">
            ›
        </span>
        ${data.name}
    </div>
</div>
`;
                }
            },
            // ACTION BUTTONS
            {
                "data": "id",
                "width": "25%",
                "render": function (data) {
                    return `
<div class="category-action-buttons">
    <a href="/Admin/Category/UpsertCategory/${data}"
       class="btn btn-success category-edit-btn">
        <i class="fas fa-edit"></i>
    </a>
    <a class="btn btn-danger category-delete-btn"
       onclick="DeleteCategory('/Admin/Category/DeleteCategory/${data}')">
        <i class="fas fa-trash-alt"></i>
    </a>
</div>
`;
                }
            }
        ],
        "createdRow": function (row, data) {
            if (data.isParentCategory) {
                $(row).addClass("parent-row");
            }
        }
    });
}
function DeleteCategory(url) {
    swal({
        title: "Are you sure?",
        text: "This category will be deleted!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        categoryTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                },
                error: function () {
                    toastr.error("Something went wrong");
                }
            });
        }
    });
}
