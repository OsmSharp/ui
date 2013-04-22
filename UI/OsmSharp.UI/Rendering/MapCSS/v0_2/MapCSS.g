grammar MapCSS;
/*
 * taken from: https://github.com/Gubaer/dart-mapcss/
 * Changes by Ben Abelshausen: target to CSharp3, Changed HIDDEN => Hidden and added parsing functions in the grammar file.
 * To generate the parser use Antlr v3 .NET tool. 
 *
 *
 * grammar for MapCSS, see http://wiki.openstreetmap.org/wiki/MapCSS/0.2
 * and collections of "real-world" mapcss files:
 *   http://code.google.com/p/kothic/source/browse/#hg%2Fsrc%2Fstyles
 *   http://josm.openstreetmap.de/wiki/Styles
 *
 * Limitations/Incompatibilities:
 *  - allows for quoted strings in double and single quotes ("..." and '...')
 *  - doesn't support the "set <...>" declaration in declaration blocks
 *
 * The grammar emits an AST.
 *
 * Identifiers
 * ===========
 *
 * MapCSS knows different types of identifier tokens: 
 *
 * - In attribute selectors identifier tokens refer to OSM tag names.
 *   There's not formal syntax, but typical examples are
 *     highway
 *     size2
 *     addr:house  (note  the ':' - in OSM it used as separator between
 *                    different "tag components")
 *     a.b         (we allow '.' as as separator, too)
 *
 *   In MapCSS tag names are also valid identifiers in declaration values,
 *   in particular in eval()-expressions.
 *
 * - Other identifiers follow the syntactical conventions of CSS, 
 *   which supports '-' in identifiers. Identifiers may even start with a '-'. 
 *   Per convention an identifier with a leading '-' is a 
 *   vendor specific extension.
 *   Typical examples are:
 *       fill-width
 *       -x-border-shading
 *   In MapCSS layer ids, class names, pseudo class names, and declaration
 *   properties are CSS identifiers. 
 *       
 */

options {
  language=CSharp3;
  output=AST;
}

tokens {
   STYLESHEET;
   RULE;
   SIMPLE_SELECTOR;
   DESCENDANT_COMBINATOR;
   CHILD_COMBINATOR;
   PARENT_COMBINATOR;
   TYPE_SELECTOR;              // .text is the type
   ZOOM_SELECTOR;              
   ATTRIBUTE_SELECTOR;
   CLASS_SELECTOR;
   ROLE_SELECTOR;
   INDEX_SELECTOR;
   PSEUDO_CLASS_SELECTOR;
   LAYER_ID_SELECTOR;         // .text is the layer id 
   DECLARATION_BLOCK;
   DECLARATION;
   
   VALUE_RGB;                  
   VALUE_RGBA;
   VALUE_URL;                  
   VALUE_KEYWORD;              // .text is the keyword, without quotes 
   VALUE_QUOTED;               // .text is the value (without quotes)
   VALUE_FLOAT;                // .text is the float value
   VALUE_INT;                  // .text is the int value 
   VALUE_PERCENTAGE;           // .text is a float or int, *with* trailing %
   VALUE_POINTS;               // .text is a float or int, *with* trailing 'pt'
   VALUE_PIXELS;               // .text is a float or int, *with* trailing 'px'
   VALUE_LIST;               
   VALUE_REGEXP;               // .text is a regular expression
   VALUE_INCREMENT;            // .text is the increment
   
   OP_EXIST;
   OP_NOT_EXIST;               // NOTE: not identical with OP_NOT -> !tag or !class
   OP_EQ='=';   
   OP_NEQ='!=';
   OP_LE='<=';
   OP_GE='>=';
   OP_LT='<';
   OP_GT='>';
   OP_MATCH='=~';
   OP_NEGATE;
   OP_STARTS_WITH='^=';
   OP_ENDS_WITH='$=';
   OP_SUBSTRING='*=';
   OP_CONTAINS='~=';
   OP_TRUTHY;
   
   OP_OR='||';
   OP_AND='&&';
   OP_NOT;                    // NOTE: boolean not -> !(expr)
   
   OP_MUL='*';
   OP_DIV;
   OP_MOD='%';
   
   OP_PLUS='+';
   OP_MINUS;
   
   FUNCTION_CALL;   
   PREDICATE;
   EVAL_CALL;   
}

@header {
 // part of mapcss;
}

@lexer::members {
    /// true, if the scanner is in a state, where OSM tag names are 
    /// valid identifiers  '[highway=residential]'
	bool isOsmTagAllowed = false;	
	/// true, if the lexer is currently in a declaration block
	bool isInDeclarationBlock = false;
}

@parser::members {
	
        internal static string _extractUrl(IToken URL110)
        {
            if (URL110 != null &&
                URL110.Text != null &&
                URL110.Text.Length > 0)
            { // there is content!
                if (URL110.Text.Substring(0, 4) != "url(")
                { // there is support to be an url( definition here!
                    throw new System.ArgumentOutOfRangeException("URL content incorect: {0}", URL110.Text);
                }

                return URL110.Text.Substring(5, URL110.Text.Length - 2 - 5);
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the blue part of a token containing a color as a number 0-255.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static string _blue(IToken c)
        {
            if (c.Text != null)
            { // there is something to parse.
                if(c.Text.Length == 7 &&
                    c.Text[0] == '#')
                { // should be a hex color.
                    int color;
                    string color_hex = "" + c.Text[5] + c.Text[6];
                    if (!int.TryParse(color_hex,
                        System.Globalization.NumberStyles.HexNumber,
                        System.Globalization.CultureInfo.InvariantCulture, out color))
                    { // parsing did not succeed. 
                        // try again an let the exception go.
                        int.Parse(color_hex,
                            System.Globalization.NumberStyles.HexNumber,
                            System.Globalization.CultureInfo.InvariantCulture);
                    }

                    // success!
                    return (color).ToString();
                }
				else if (c.Text.Length == 4 &&
                    c.Text[0] == '#')
                { // should be a hex color.
                    byte color;
                    string color_hex = "" + c.Text[3] + c.Text[3];
                    if (!byte.TryParse(color_hex, 
                        System.Globalization.NumberStyles.HexNumber,
                        System.Globalization.CultureInfo.InvariantCulture, out color))
                    { // parsing did not succeed. 
                        // try again an let the exception go.
                        int.Parse(color_hex,
                            System.Globalization.NumberStyles.HexNumber,
                            System.Globalization.CultureInfo.InvariantCulture);
                    }

                    // success!
                    return (color).ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the green part of a token containing a color as a number 0-255.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static string _green(IToken c)
        {
            if (c.Text != null)
            { // there is something to parse.
                if (c.Text.Length == 7 &&
                    c.Text[0] == '#')
                { // should be a hex color.
                    byte color;
                    string color_hex = "" + c.Text[3] + c.Text[4];
                    if (!byte.TryParse(color_hex,
                        System.Globalization.NumberStyles.HexNumber,
                        System.Globalization.CultureInfo.InvariantCulture, out color))
                    { // parsing did not succeed. 
                        // try again an let the exception go.
                        int.Parse(color_hex,
                            System.Globalization.NumberStyles.HexNumber,
                            System.Globalization.CultureInfo.InvariantCulture);
                    }

                    // success!
                    return (color).ToString();
                }
				else if (c.Text.Length == 4 &&
                    c.Text[0] == '#')
                { // should be a hex color.
                    byte color;
                    string color_hex = "" + c.Text[2] + c.Text[2];
                    if (!byte.TryParse(color_hex, 
                        System.Globalization.NumberStyles.HexNumber,
                        System.Globalization.CultureInfo.InvariantCulture, out color))
                    { // parsing did not succeed. 
                        // try again an let the exception go.
                        int.Parse(color_hex,
                            System.Globalization.NumberStyles.HexNumber,
                            System.Globalization.CultureInfo.InvariantCulture);
                    }

                    // success!
                    return (color).ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the red part of a token containing a color as a number 0-255.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static string _red(IToken c)
        {
            if (c.Text != null)
            { // there is something to parse.
                if (c.Text.Length == 7 &&
                    c.Text[0] == '#')
                { // should be a hex color.
                    byte color;
                    string color_hex = "" + c.Text[1] + c.Text[2];
                    if (!byte.TryParse(color_hex, 
                        System.Globalization.NumberStyles.HexNumber,
                        System.Globalization.CultureInfo.InvariantCulture, out color))
                    { // parsing did not succeed. 
                        // try again an let the exception go.
                        int.Parse(color_hex,
                            System.Globalization.NumberStyles.HexNumber,
                            System.Globalization.CultureInfo.InvariantCulture);
                    }

                    // success!
                    return (color).ToString();
                }
				else if (c.Text.Length == 4 &&
                    c.Text[0] == '#')
                { // should be a hex color.
                    byte color;
                    string color_hex = "" + c.Text[1] + c.Text[1];
                    if (!byte.TryParse(color_hex, 
                        System.Globalization.NumberStyles.HexNumber,
                        System.Globalization.CultureInfo.InvariantCulture, out color))
                    { // parsing did not succeed. 
                        // try again an let the exception go.
                        int.Parse(color_hex,
                            System.Globalization.NumberStyles.HexNumber,
                            System.Globalization.CultureInfo.InvariantCulture);
                    }

                    // success!
                    return (color).ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Get the higher zoom.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        internal static string _zoomUpper(IToken v)
        {
            if (v != null &&
                v.Text != null)
            { // there is content in the token.
                if (v.Text[0] != 'z')
                { // first character verified.
                    string zoom_string = new string(v.Text.ToCharArray(),
                        2, v.Text.Length - 2);
                    string[] zooms = zoom_string.Split('-');
                    if (zooms.Length == 1)
                    { // there is just one zoom, return it as the lower zoom!
                        return zooms[0];
                    }
                    else if (zooms.Length == 2)
                    { // get just the lower zoom.
                        int zoom;
                        if (zooms[1] != null &&
                            zooms[1].Length > 0)
                        { // there is content!
                            if (int.TryParse(zooms[1], out zoom))
                            { // zoom value.
                                return zooms[1];
                            } // invalid zoom string.
                            throw new System.ArgumentOutOfRangeException(string.Format("Zoom string {0} cannot be parsed!",
                                v.Text));
                        }
                        return zooms[1];
                    }
                    else
                    { // invalid zoom string.
                        throw new System.ArgumentOutOfRangeException(string.Format("Zoom string {0} cannot be parsed!",
                            v.Text));
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Get the lower zoom.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        internal static string _zoomLower(IToken v)
        {
            if (v != null &&
                v.Text != null)
            { // there is content in the token.
                if (v.Text[0] != 'z')
                { // first character verified.
                    string zoom_string = new string(v.Text.ToCharArray(),
                        2, v.Text.Length - 2);
                    string[] zooms = zoom_string.Split('-');
                    if (zooms.Length == 1)
                    { // there is just one zoom, return it as the lower zoom!
                        return zooms[0];
                    }
                    else if (zooms.Length == 2)
                    { // get just the lower zoom.
                        int zoom;
                        if (zooms[0] != null &&
                            zooms[0].Length > 0)
                        { // there is content!
                            if (int.TryParse(zooms[0], out zoom))
                            { // zoom value.
                                return zooms[0];
                            } // invalid zoom string.
                            throw new System.ArgumentOutOfRangeException(string.Format("Zoom string {0} cannot be parsed!",
                                v.Text));
                        }
                        return zooms[0];
                    }
                    else
                    { // invalid zoom string.
                        throw new System.ArgumentOutOfRangeException(string.Format("Zoom string {0} cannot be parsed!",
                            v.Text));
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Unquote the given text.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        internal static string _unquote(IToken v)
        {
            if (v != null &&
                v.Text != null)
            { // there is content in the token.
                if (v.Text.Length < 2)
                { // how can this be? there are supposed to be quoutes!
                    throw new System.ArgumentOutOfRangeException("Token must have at least two characters!");
                }
                else if (v.Text[0] != '"' && v.Text[0] != '\'')
                { // how can this be? there are supposed to be quotes!
                    throw new System.ArgumentOutOfRangeException("Token must have a starting quote!");
                }
                else if (v.Text[v.Text.Length - 1] != '"' && v.Text[v.Text.Length - 1] != '\'')
                { // how can this be? there are supposed to be quotes!
                    throw new System.ArgumentOutOfRangeException("Token must have a starting quote!");
                }
                else
                { // there are two quotes at the beginning and the end.
                    return new string(v.Text.ToCharArray(), 1, v.Text.Length - 2);
                }
            }
            return string.Empty;
        }
}

fragment EBACKSLASH: '\\\\';
fragment UNICODE: '\u0080'..'\uFFFF';
 

RGB: ('r' | 'R') ('g' | 'G') ('b' | 'B');
RGBA: ('r' | 'R') ('g' | 'G') ('b' | 'B') ('a' | 'A');
ROLE: ('r' | 'R') ('o' | 'O') ('l' | 'L') ('e' | 'E');
INDEX: ('i' | 'I') ('n' | 'N') ('d' | 'D') ('e' | 'E') ('x' | 'X');
EVAL: ('e' | 'E') ('v' | 'V') ('a' | 'A') ('l' | 'L');
LIST: ('l' | 'L') ('i' | 'I') ('s' | 'S') ('t' | 'T');
IMPORT: '@' ('i' | 'I') ('m' | 'M') ('p' | 'P') ('o' | 'O')('r' | 'R') ('t' | 'T');

fragment HWS: (' ' | '\t' | '\f');
fragment URLCONTENT: ('!' | '#' | '$' | '%' | '&' | '*'..'[' | ']'..'~' | NONASCII)+;
URL: ('u' | 'U') ('r' | 'R') ('l' | 'L') HWS* '(' HWS* 
     (
	         URLCONTENT
	   | '"' URLCONTENT '"'  
	   | '\'' URLCONTENT '\''
     )
     HWS* ')';

fragment DIGIT:  '0'..'9';
fragment CHAR: 'a'..'z' | 'A'..'Z';


/* Basic character sets from CSS specification */
fragment NONASCII: ~('\u0000' .. '\u009F');                     
fragment NMSTART: 'a'..'z' | 'A'..'Z' | '_' | NONASCII;
fragment NMCHAR: 'a'..'z' | 'A'..'Z' | '_' | '-' | NONASCII;

/* helpers */
fragment NCOMPONENT: (CHAR | '_') (CHAR | DIGIT | '_' | '-')*;
fragment TAGSEPARATOR: (':') | ('.');

/* context specific lexer for identifiers. Recognizes CSS idents and OSM tag names */
fragment CSS_IDENT:;
fragment OSM_TAG:;
IDENTS:
	'-' ?  NCOMPONENT (
	      {isOsmTagAllowed}? =>  (
	          (TAGSEPARATOR) =>  TAGSEPARATOR NCOMPONENT (TAGSEPARATOR NCOMPONENT)*  {$type=OSM_TAG;}
	        | {$type=CSS_IDENT;}	                    
	      )
	   |  {$type=CSS_IDENT;}
	); 	

/*
 * OSM tag names are allowed in attribute and link selectors 
 */
LBRACKET 
  @after{isOsmTagAllowed=true;}
  : '[';
  
 RBRACKET
  @after{isOsmTagAllowed=false;}
  : ']'; 

/*
 * OSM tag names are allowed in declaration values, but not in declaration
 * properties. We enable them, after we've scanned a '{' and then a ':'
 * separating the declaration property from the declaration value.
 */

LBRACE
  @after{
    isInDeclarationBlock=true;
    isOsmTagAllowed=false;
  }
  : '{';
  
RBRACE
  @after{
    isInDeclarationBlock=false;
    isOsmTagAllowed=false;
  }
  : '}'; 
  
COLON
  @after{
    isOsmTagAllowed=isInDeclarationBlock;
  }
  : ':';   
  
SEMICOLON
  @after{
    isOsmTagAllowed=false;
  }
  : ';';

/* -------------------- quoted strings -----------------------------------------------------------*/
fragment EDQUOTE: '\\"';
fragment ESQUOTE: '\\\'';
DQUOTED_STRING: '"' (' ' | '!' | '#'..'[' | ']'..'~' | UNICODE | EDQUOTE | EBACKSLASH )* '"';
SQUOTED_STRING: '\'' (' '..'&' | '('..'[' | ']'..'~' | UNICODE | ESQUOTE | EBACKSLASH)* '\'';

/* -------------------- hex color constants -------------------------------------------------------*/
fragment HEXDIGIT: DIGIT | 'a'..'f' | 'A'..'F'; 
HEXCOLOR: '#' ((HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT) | (HEXDIGIT HEXDIGIT HEXDIGIT));

/* ----------------------------------------------------------------------------------------------- */
/* Positive/Negative numbers and numbers with units  (derived from an ANTLR grammar for            */
/* CSS 2.1)                                                                                        */
/* ----------------------------------------------------------------------------------------------- */
fragment PERCENTAGE:;
fragment PIXELS:;
fragment POINTS:;
fragment POSITIVE_FLOAT:;
fragment POSITIVE_INT:;
fragment NEGATIVE_FLOAT:;
fragment NEGATIVE_INT:;
fragment INCREMENT:;
fragment P: ('p' | 'P');
fragment T: ('t' | 'T');
fragment X: ('x' | 'X');


NUMBER
	: ('-'? DIGIT* ('.' DIGIT+)?) => s='-'? DIGIT* (d='.' DIGIT+)?
	    (
		   (P (T | X)) => 
			  P
			  (
				   T              {$type = POINTS;}
			     | X              {$type = PIXELS;}
		  	)
	      | ('%') => '%'          {$type = PERCENTAGE;}	    
	      | 
	        {
	           if ($s == null) {
	              $type = ($d == null ? POSITIVE_INT : POSITIVE_FLOAT);
	           } else {
	              $type = ($d == null ? NEGATIVE_INT : NEGATIVE_FLOAT);
	           }
   	        }
	  )	 	
	| ('+') => '+' DIGIT+         {$type = INCREMENT;}
	;		

/* ----------------------------------------------------------------------------------------------- */
/* Zoom range                                                                                      */
/* ----------------------------------------------------------------------------------------------- */
RANGE
	: '|z' (
		  '-' DIGIT+
		| DIGIT+ 
		| DIGIT+ '-' 
		| DIGIT+ '-' DIGIT+
	  )
	;
	
/* ----------------------------------------------------------------------------------------------- */
/* Regular expressions  and the '/' operator                                                       */
/* ----------------------------------------------------------------------------------------------- */
fragment REGEX_ESCAPE:   '\\\\' | '\\/' | '\\(' | '\\)' 
                       | '\\|' | '\\$' | '\\*' | '\\.' | '\\^' | '\\?' | '\\+' | '\\-'
                       | '\\n' | '\\r' | '\\t'
                       | '\\s' | '\\S'
                       | '\\d' | '\\D'
                       | '\\w' | '\\W';   
fragment REGEX_START:  ' '..')' | '+'..'.' |'0'..'[' | ']'..'~' | UNICODE | REGEX_ESCAPE;
fragment REGEX_CHAR:  ' '..'.' |'0'..'[' | ']'..'~' | UNICODE | REGEX_ESCAPE;
   
                       
/*
 * make sure a / is  followed by a regular expression. If not, treat it
 * as DIV operator.
 */ 
fragment DIV:;   
REGEXP:  '/'  (
              (REGEX_START REGEX_CHAR* '/') 
                    => REGEX_START REGEX_CHAR* '/'   {$type=REGEXP;}
           |  (.)   =>                               {$type=DIV;}
         );

/* ----------------------------------------------------------------------------------------------- */
/* Whitespace and comments                                                                         */
/* ----------------------------------------------------------------------------------------------- */
WS:		    (' ' | '\t' | '\n' | '\r' | '\f') {$channel=Hidden;}; 
SL_COMMENT:   '//' (options {greedy=false;}: .)* '\r'? '\n' {$channel=Hidden;};
ML_COMMENT:   '/*'  (options {greedy=false;} : .)* '*/' {$channel=Hidden;};


/* =============================================================================================== */
/* Grammar                                                                                         */
/* ===============================================================================================  */

public stylesheet
	: entry* EOF -> ^(STYLESHEET entry*)
	;

entry
	: rule
	| import_statement
	;
			
rule
    /*
     * we allow an arbitray number of "empty" selectors seperated by ',' before a declaration block
     */
	: selector (',' selector)* ','* declaration_blocks -> ^(RULE selector* declaration_blocks)
	;
  
selector
	: simple_selector                     -> simple_selector
	| simple_selector simple_selector     -> ^(DESCENDANT_COMBINATOR simple_selector+)
	| simple_selector '>' link_selector*  simple_selector -> ^(CHILD_COMBINATOR simple_selector+ link_selector*)
	| simple_selector '<' simple_selector -> ^(PARENT_COMBINATOR simple_selector+)
	;

link_selector
	: LBRACKET ROLE binary_operator predicate_primitive RBRACKET  -> ^(ROLE_SELECTOR binary_operator predicate_primitive)
	| LBRACKET INDEX op=int_operator v=POSITIVE_INT RBRACKET   -> ^(INDEX_SELECTOR int_operator VALUE_INT[v])
	;

layer_id_selector
	: '::' k=CSS_IDENT -> LAYER_ID_SELECTOR[$k]
	;
	
int_operator : OP_EQ | OP_NEQ | OP_LT| OP_LE| OP_GT| OP_GE;
		
import_statement
	: IMPORT URL '(' url=quoted ')' id=CSS_IDENT ';' -> ^(IMPORT VALUE_URL[$url.text] VALUE_KEYWORD[$id])
	;

simple_selector
	: type_selector class_selector? zoom_selector? attribute_selector* pseudo_class_selector* layer_id_selector?
	     -> ^(SIMPLE_SELECTOR type_selector class_selector? zoom_selector? attribute_selector* pseudo_class_selector* layer_id_selector?)
	;

zoom_selector
	: v=RANGE -> ^(ZOOM_SELECTOR VALUE_INT[_zoomLower($v)] VALUE_INT[_zoomUpper($v)])
	;

quoted
	: v=DQUOTED_STRING   -> VALUE_QUOTED[_unquote($v)]
	| v=SQUOTED_STRING   -> VALUE_QUOTED[_unquote($v)]
	; 
	
cssident
	: v=CSS_IDENT   -> VALUE_KEYWORD[$v]
	;	

attribute_selector
	: LBRACKET  predicate RBRACKET   -> ^(ATTRIBUTE_SELECTOR predicate)
	;

lhs
	: quoted 
	| k=CSS_IDENT  -> VALUE_KEYWORD[$k]
    | k=OSM_TAG    -> VALUE_KEYWORD[$k]
	;
		
predicate
	: predicate_ident                     -> OP_EXIST predicate_ident
	| quoted                     		  -> OP_EXIST quoted
	| predicate_primitive binary_operator predicate_primitive ->   binary_operator predicate_primitive+  
	| predicate_ident OP_MATCH rhs_match  -> OP_MATCH  predicate_ident  rhs_match
	| '!' predicate_ident                 -> OP_NOT_EXIST predicate_ident
	| predicate_ident '?'                 -> OP_TRUTHY predicate_ident
	| '!' quoted                 		  -> OP_NOT_EXIST quoted
	| quoted '?'                		  -> OP_TRUTHY quoted	
	| p=CSS_IDENT '(' quoted ')'          -> PREDICATE[$p] quoted  
	;

predicate_ident
	: cssident
	| k=OSM_TAG   -> VALUE_KEYWORD[$k]
	;
	
predicate_primitive
	: num	
	| predicate_ident
	| quoted
	;
	
rhs_match
	: quoted
	| r=REGEXP                     -> VALUE_REGEXP[$r]	
	;
	
binary_operator
	: OP_EQ | OP_NEQ | OP_LT | OP_GT | OP_LE
	| OP_GE | OP_STARTS_WITH | OP_ENDS_WITH | OP_SUBSTRING
    | OP_CONTAINS
	;        

class_selector
	: '!.'  cssident  -> ^(CLASS_SELECTOR OP_NOT_EXIST cssident)
	|  '.'  cssident  -> ^(CLASS_SELECTOR OP_EXIST     cssident)
	;

pseudo_class_selector
    : ':!' cssident   -> ^(PSEUDO_CLASS_SELECTOR OP_NOT_EXIST cssident) 
    | '!:' cssident   -> ^(PSEUDO_CLASS_SELECTOR OP_NOT_EXIST cssident)
	| ':' cssident   -> ^(PSEUDO_CLASS_SELECTOR OP_EXIST cssident)
	;	

type_selector
    /*
     * In the AST we allow any ident as type name
     */
    : v=CSS_IDENT    -> TYPE_SELECTOR[$v]
    | v='*'          -> TYPE_SELECTOR[$v]
	;

declaration_blocks
	: declaration_block (declaration_block)* -> declaration_block*
	;

declaration_block
    /*
     * we set the token text to '{' in order to keep track of line and 
     * column informatin. 
     */
	:  l=LBRACE declarations RBRACE -> ^(DECLARATION_BLOCK[$l] declarations)
	|  l=LBRACE RBRACE              -> ^(DECLARATION_BLOCK[$l])
	;

declarations
	: declaration (SEMICOLON declaration)* ';'*  -> declaration*
	;

declaration
	: declaration_property COLON declaration_value  -> ^(DECLARATION declaration_property declaration_value)
	;

declaration_property
	: cssident
	;	

declaration_value
	: single_value
	| single_value  ',' single_value (',' single_value)* -> ^(VALUE_LIST single_value*)
	| EVAL  '(' expr ')'  -> ^(EVAL_CALL expr)	
	| LIST  '(' single_value (',' single_value)* ')' -> 	^(VALUE_LIST single_value*)	
	| LIST  '(' ')' -> ^(VALUE_LIST)
	;
 

num
	: n=POSITIVE_INT       ->   VALUE_INT[$n]
	| n=NEGATIVE_INT       ->   VALUE_INT[$n]
	| n=POSITIVE_FLOAT     ->   VALUE_FLOAT[$n]
	| n=NEGATIVE_FLOAT     ->   VALUE_FLOAT[$n]
	;

single_value	
	: v=POSITIVE_INT            -> VALUE_INT[$v]
	| v=NEGATIVE_INT            -> VALUE_INT[$v]
	| v=POSITIVE_FLOAT          -> VALUE_FLOAT[$v]
	| v=NEGATIVE_FLOAT          -> VALUE_FLOAT[$v]	
	| v=INCREMENT               -> VALUE_INCREMENT[$v]
	| v=POINTS 		   -> VALUE_POINTS[$v]
	| v=PIXELS         -> VALUE_PIXELS[$v]
	| v=PERCENTAGE     -> VALUE_PERCENTAGE[$v] 
	| URL   -> VALUE_URL[_extractUrl($URL)]
    | RGB '(' r=POSITIVE_INT ',' g=POSITIVE_INT ',' b=POSITIVE_INT ')'            
	         -> ^(VALUE_RGB VALUE_INT[$r] VALUE_INT[$g] VALUE_INT[$b])	
	| RGBA '(' r=POSITIVE_INT ',' g=POSITIVE_INT ',' b=POSITIVE_INT ',' a=num ')'
	         -> ^(VALUE_RGBA VALUE_INT[$r] VALUE_INT[$g] VALUE_INT[$b] VALUE_FLOAT[$a.text])
	| c=HEXCOLOR      -> ^(VALUE_RGB VALUE_INT[_red(c)] VALUE_INT[_green(c)] VALUE_INT[_blue(c)])
	| quoted           -> VALUE_QUOTED[$quoted.text]

    /* make sure these are the last alternatives in this rule */
    | k=OSM_TAG            -> VALUE_KEYWORD[$k]    	 
    | cssident   
	;
	
/* ------------------------------------------------------------------------------------------ */
/* eval expressions                                                                           */
/* ------------------------------------------------------------------------------------------ */
expr
	: logicalExpression
	;

args
	: expr (',' expr)*    ->  expr+
	; 

logicalExpression
    :    booleanAndExpression ( 
             (OP_OR) => OP_OR logicalExpression -> ^(OP_OR booleanAndExpression logicalExpression)
           |                                -> booleanAndExpression
         )
    ;
    
booleanAndExpression
    :    equalityExpression ( 
             (OP_AND) => OP_AND booleanAndExpression -> ^(OP_AND equalityExpression booleanAndExpression)
           |                                     -> equalityExpression
         )
    ;
         	
equalityExpression
    :    relationalExpression (
             (OP_EQ)  => OP_EQ  relationalExpression -> ^(OP_EQ  relationalExpression+)
           | (OP_NEQ) => OP_NEQ relationalExpression -> ^(OP_NEQ relationalExpression+)
           |                                     -> relationalExpression
         )
    ;
             	
relationalExpression
    :   additiveExpression ( 
            (OP_LT) =>  OP_LT additiveExpression  -> ^(OP_LT additiveExpression+)
          | (OP_LE) =>  OP_LE additiveExpression  -> ^(OP_LE additiveExpression+)
          | (OP_GT) =>  OP_GT additiveExpression  -> ^(OP_GT additiveExpression+)
          | (OP_GE) =>  OP_GE additiveExpression  -> ^(OP_GE additiveExpression+)
          |                                 -> additiveExpression
        )
    ;

additiveExpression
    :    multiplicativeExpression ( 
             (OP_PLUS) => OP_PLUS  additiveExpression -> ^(OP_PLUS multiplicativeExpression additiveExpression)
           | ('-') => '-'  additiveExpression -> ^(OP_MINUS multiplicativeExpression additiveExpression)
           |                                  -> multiplicativeExpression
         )
    ;

multiplicativeExpression
    :  unaryExpression (
         (OP_MUL) => (OP_MUL multiplicativeExpression) -> ^(OP_MUL unaryExpression multiplicativeExpression)
       | (DIV) => (DIV multiplicativeExpression) -> ^(OP_DIV unaryExpression multiplicativeExpression)
       | (OP_MOD) => (OP_MOD multiplicativeExpression) -> ^(OP_MOD unaryExpression multiplicativeExpression)
       |                                         -> unaryExpression
    )                    
    ;

unaryExpression
    :       ('!') => '!' primaryExpression  -> ^(OP_NOT primaryExpression)
         |  primaryExpression               -> primaryExpression
    ;

primaryExpression
    :    '(' expr ')'      -> expr
    |    f=CSS_IDENT '(' args? ')' -> ^(FUNCTION_CALL[$f] args?)
    |    v=POSITIVE_FLOAT  ->  VALUE_FLOAT[$v]
    |    v=POSITIVE_INT    ->  VALUE_INT[$v]
    |    v=NEGATIVE_FLOAT  ->  VALUE_FLOAT[$v]
    |    v=NEGATIVE_INT    ->  VALUE_INT[$v]
    |    quoted
    |    cssident
    |    v=OSM_TAG   -> VALUE_KEYWORD[$v]
    ;
                 	


