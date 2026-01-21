
window.addEventListener('load', function () {
  
    const el = document.getElementById('swal-messages');
    if (!el) return; // No messages

    const successMsg = el.dataset.success;
    const errorMsg = el.dataset.error;

    if (successMsg) {
        Swal.fire({
            title: 'Success',
            text: successMsg,
            icon: 'success',
            confirmButtonText: 'OK'
        });
        return;
    }

    if (errorMsg) {
        Swal.fire({
            title: 'Error',
            text: errorMsg,
            icon: 'error',
            confirmButtonText: 'OK'
        });
    }
});
