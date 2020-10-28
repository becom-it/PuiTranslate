
class TranslationListJsHelper {

    Initialize() {
        /** @type {NodeList} */
        let rows = document.querySelectorAll(".tr-mainrow");
        rows.forEach((r) => {
            r.onclick = (evt) => {
                if (evt.currentTarget) {
                    if (evt.currentTarget.nextElementSibling) {
                        /** @type {HTMLTableRowElement} */
                        let nextrow = evt.currentTarget.nextElementSibling;
                        if (nextrow.classList.contains("tab-hidden")) {
                            nextrow.classList.remove("tab-hidden");
                        } else {
                            nextrow.classList.add("tab-hidden");
                        }
                    }
                }
            }
        });
    }
}

window.translationListJsHelper = new TranslationListJsHelper();