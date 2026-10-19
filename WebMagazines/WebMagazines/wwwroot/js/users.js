var userDataTable;

$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
    userDataTable = $('#tblData').DataTable({
        ajax: '/user/getall',
        columns: [
            {
                data: 'name',
                "width": "15%",
                "render": function (data) {
                    return `
            <i class="bi bi-person me-2"></i>
            ${data}
        `;
                }
            },
            { data: 'email', "width": "20%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'state', "width": "10%" },
            {
                data: 'role', "width": "10%", "render": function (data) { return '<span class="badge bg-secondary">' + data + '</span>'; }
            },
            {
                data: { id: "id", lockoutEnd:"lockoutEnd"}, "width": "35%", "render": function (data) {

                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    var isLocked = lockout > today;


return `
    <div class="user-action-buttons d-flex justify-content-end align-items-center gap-2 flex-wrap">

        <button type="button"
                onclick="LockUnlock('${data.id}')"
                class="btn btn-sm user-action-btn ${isLocked ? 'user-action-unlock' : 'user-action-lock'}">
            <i class="bi bi-${isLocked ? 'unlock-fill' : 'lock-fill'}"></i>
            <span>${isLocked ? 'Unlock' : 'Lock'}</span>
        </button>

        <a href="/user/RoleManagement?userId=${encodeURIComponent(data.id)}"
           class="btn btn-sm user-action-btn user-action-role">
            <i class="bi bi-person-badge-fill"></i>
            <span>Role</span>
        </a>

        <a href="/user/ChangePassword?userId=${encodeURIComponent(data.id)}"
           class="btn btn-sm user-action-btn user-action-password">
            <i class="bi bi-key-fill"></i>
            <span>Change Password</span>
        </a>

    </div>`;

                }
            }

        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/user/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                userDataTable.ajax.reload();
            }
        }
    });
}