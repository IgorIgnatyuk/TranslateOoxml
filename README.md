# TranslateOoxml

This application translates DOCX, PPTX and XLSX files using DeepL API (https://www.deepl.com/docs-api). You will need a DeepL API authentication key to use it.

Only the DeepL text-translation API and not the DeepL document translation API is used. That saves translation costs because with the document translation API “you are billed a minimum of 50,000 characters with the DeepL API plan, no matter how many characters are included in the document”, and allows translation of XLSX files that is not supported by the DeepL document translation API currently.



    Syntax: TranslateOoxml sourceFile targetLanguage

    The source file can be a .docx, .pptx, or .xlsx one.

    The target file will appear in the same folder where the source file resides.
    The target file name will have the target language code as a suffix.

    Language codes: https://www.deepl.com/docs-api/translate-text/translate-text/

    The environment variable DEEPL_AUTH_KEY should be set.



The following files in this folder are machine translations to German:

    Info_DE.docx
    Info_DE.pptx
    Info_DE.xlsx
