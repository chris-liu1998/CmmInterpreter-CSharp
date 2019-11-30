# CmmInterpreter  

## C#版CMM解释器

### 语法分析

ll(1)文法  
>
> NO FUNCTION\ELSE_IF\POINTER YET（Maybe later）
>
> + program -> stmt-sequence  
> + stmt-sequence -> statement stmt-sequence | ε
> + statement -> if-stmt | while-stmt | assign-stmt | scan-stmt | print-stmt | declare-stmt | stmt-block | jump-stmt | ;  
> + stmt-block -> { stmt-sequence }  
> + if-stmt -> if ( assign-exp ) if-block  
> + if-block -> statement else-stmt | else-stmt
> + else-stmt -> else statement | ε
> + while-stmt -> while ( assign-exp ) statement  
> + jump-stmt -> break; | continue;  
> + assign-stmt -> assign-exp ;  
> + assign-exp -> logic-or-exp more-logic-or-exp
> + more-logic-or-exp -> assign-op logic-or-exp  more-logic-or-exp | ε  
> + scan-stmt -> scan ( variable ) ;  
> + print-stmt -> print ( logic-or-exp ) ;  
> + declare-stmt -> (int | real | char) variable-list;  
> + initializer -> = value | ε  
> + value -> logic-or-exp | { value-list }
> + value-list -> value more-value  
> + more-value -> , value more-value | ε  
> + variable -> identifier array-dim
> + array-dim -> [logic-or-exp] array-dim | ε  
> + variable-list -> variable initializer more-variables  
> + more-variables -> , variable initializer more-variables| ε  
> + logic-or-exp -> logic-and-exp more-logic-and-exp  
> + more-logic-and-exp -> || logic-and-exp more-logic-and-exp | ε  
> + logic-and-exp -> comp-eq-exp more-comp-eq-exp  
> + more-comp-eq-exp -> && comp-eq-exp more-comp-eq-exp | ε  
> + comp-eq-exp -> comp-exp more-comp-exp  
> + more-comp-exp -> comp-eq-op comp-exp more-comp-exp | ε  
> + comp-exp -> additive-exp more-additive-exp  
> + more-additive-exp -> comp-op additive-exp more-additive-exp | ε  
> + additive-exp -> term more-term  
> + more-term -> add-op additive-exp more-term | ε  
> + term -> factor more-factor  
> + more-factor -> mul-op term | ε  
> + factor -> specific optr | !factor | inc-op factor | Add-op factor  
> + specific -> ( logic-or-exp ) | number | variable | NULL | char-value | scan( variable )  
> + optr -> inc-op | ε  
> + inc-op -> ++ | --  
> + number -> int-val | real-val  
> + comp-eq-op -> <> | ==  
> + comp-op -> > | < | >= | <=  
> + add-op -> + | -  
> + mul-op -> * | / | %  
> + assign-op -> = | += | -= | *= | /= | %=  
