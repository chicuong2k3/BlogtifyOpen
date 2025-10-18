export function updateCodeBlock(isDark) {
    const codeBlockLink = document.getElementById("code-block-stylesheet");
    if (codeBlockLink) {
        if (!isDark) {
            codeBlockLink.href = `css/prism-coy-without-shadows.min.css`;
        }
        else {
            codeBlockLink.href = `css/prism-vsc-dark-plus.min.css`;
        }
    }
}