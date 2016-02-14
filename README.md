WiktionaryMapper 
================ 
 
WiktionaryMapper is C# project that allows to **extract** data from 
[Wiktionary](en.wiktionary.org/) [dumps](http://dumps.wikimedia.org/enwiktionary/) 
and remap into more suitable form such as *SQL*, *XML*, *JSON* or simple text files.
It is **not** a Wikipedia Dump to SQL parser.

Main Features 
------------- 
WiktionaryMapper was designed to handle large XML dumps and uses streaming to read and write data.
Output object can take any type.

Example output 
-------------- 
Example output from [Dictionary definition](http://en.wiktionary.org/w/index.php?title=dictionary) page.
Mapper was configured to seek for synonyms. 
Output was configured to SQL (specifically SQLite).
```SQL 
CREATE TABLE wiki_Synonyms (ExpressionA VARCHAR NOT NULL,ExpressionB VARCHAR NOT NULL,Language VARCHAR NOT NULL); 
INSERT INTO wiki_Synonyms VALUES ('dictionary','wordbook','en'); 
INSERT INTO wiki_Synonyms VALUES ('free','free of charge','en'); 
INSERT INTO wiki_Synonyms VALUES ('free','gratis','en'); 
.... 
``` 

Sub Projects 
------------ 
WiktionaryMapper contains three sub projects 
* WiktionaryParser (dll)
* WiktionaryMapper (exe)
* SqliteBatchImport (exe)

**WiktionaryParser** is responsible for low level operations like page parsing or section detections.
It is unlikely you will have to modify any part of this library.
It is fairly generic and can be used to parse other Wikipedia pages.  
**WiktionaryMapper** is responsible for mapping sections string to desired objects.  
**SqliteBatchImport** is an add-on which allows to import large amount of SQL to SQLite database efficiently in transactional fashion.  
 
Performance 
----------- 
3 GB dump form English Wiktionary was remapped to synonyms, definitions and translations SQL files in roughly 6 minutes.  
Tested on: Windows 8.1, Intel i5-2520, Samsung SSD 840 while watching video on YouTube. 
 
Credits 
------- 
* WiktionaryMapper is part of [Memoling](http://memoling.com) project. 
* SqlBatchImport uses components from [System.Data.SQLite](http://system.data.sqlite.org/). 
