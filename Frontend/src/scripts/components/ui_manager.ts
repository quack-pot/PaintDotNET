import { UIState } from "../types/ui_states";

interface UIPanel {
    state: UIState;
    element: HTMLElement;
}

export default class UIManager extends HTMLElement {
    private state: UIState = UIState.MAIN_MENU;

    private panels: UIPanel[] = [];

    constructor() {
        super();

        for (let idx: number = 0; idx < this.children.length; ++idx) {
            const child: Element | null = this.children.item(idx);

            if (child === null) {
                continue;
            }

            const ui_state: string | null = child.getAttribute("data-ui-state");

            if (ui_state === null) {
                continue;
            }

            this.panels.push({
                state: Number(ui_state) as UIState,
                element: child as HTMLElement
            });
        }

        this.updateState(this.state);
    }

    public updateState(new_state: UIState): void {
        this.state = new_state;

        for (let idx: number = 0; idx < this.panels.length; ++idx) {
            const panel: UIPanel = this.panels[idx];

            if (new_state == panel.state) {
                panel.element.style.removeProperty("display");
                continue;
            }

            panel.element.style.setProperty("display", "none");
        }
    }

    public getState(): UIState { return this.state; }
}

customElements.define("ui-manager", UIManager);