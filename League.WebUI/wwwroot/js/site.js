document.querySelectorAll('[data-autosubmit]').forEach(select => {
    select.addEventListener('change', () => select.form.requestSubmit());
});
const statusSelect = document.getElementById('result-status');
if (statusSelect) {
    const update = () => {
        document.querySelectorAll('[data-score]').forEach(input => {
            input.required = statusSelect.value !== '0';
        });
    };
    statusSelect.addEventListener('change', update);
    update();
}
