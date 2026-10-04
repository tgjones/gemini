# AvalonDock 4.74.1 complete-state fixture

**Intent:** preserve an immutable pre-v5 Gemini whole-state input so future
docking migrations must prove forward import and binary rollback without
rewriting the only compatible source state.

`ApplicationState.bin` was written from Gemini commit
`15cdb9afad39c7fd8ddaf732b426f9eb9c21719e` with AvalonDock 4.74.1. It contains
Gemini's binary item envelope and payloads followed by the serializer's XML for
two documents and six tools spanning docked, floating, and hidden state.

SHA-256:
`58FF3ED46BD9E545C15D8A9B81CD9475CC404B3F690D7EE326FD80C40DBBB7D4`.

Do not modify or regenerate this file. Add a separate writer-version directory
for future fixtures.
