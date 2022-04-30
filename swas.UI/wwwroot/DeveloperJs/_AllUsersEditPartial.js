function ValInDataEdit(input) {
    let regex = /[^a-zA-Z0-9_]/g; // underscore allowed
    input.value = input.value.replace(regex, "");
}

