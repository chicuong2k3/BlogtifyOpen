﻿window.addCopyButtons = function () {
    const codeBlocks = document.querySelectorAll('pre');

    codeBlocks.forEach((block) => {
        if (block.parentElement.classList.contains("code-block-wrapper"))
            return;

        const wrapper = document.createElement("div");
        wrapper.className = "code-block-wrapper";
        block.parentNode.insertBefore(wrapper, block);
        wrapper.appendChild(block);

        if (block.querySelector(".copy-code-btn"))
            return;

        const code = block.querySelector("code")?.textContent || block.textContent;
        const button = document.createElement('button');
        button.innerText = "Copy";
        button.className = "copy-code-btn bit-btn bit-btn-ntx bit-btn-fil bit-btn-pri bit-btn-md";

        button.onclick = () => {
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(code).then(() => {
                    button.innerText = "Copied!";
                    setTimeout(() => button.innerText = "Copy", 1500);
                });
            } else {
                const textarea = document.createElement("textarea");
                textarea.value = code;
                document.body.appendChild(textarea);
                textarea.select();
                try {
                    document.execCommand("copy");
                    button.innerText = "Copied!";
                } catch (err) {
                }
                document.body.removeChild(textarea);
                setTimeout(() => button.innerText = "Copy", 1500);
            }
        };

        wrapper.appendChild(button);
    });
}
