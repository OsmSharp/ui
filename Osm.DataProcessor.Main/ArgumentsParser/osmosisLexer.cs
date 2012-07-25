//// $ANTLR 3.3 Nov 30, 2010 12:45:30 C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g 2011-05-10 12:47:07

//// The variable 'variable' is assigned but its value is never used.
//#pragma warning disable 168, 219
//// Unreachable code detected.
//#pragma warning disable 162


//using System;
//using System.Collections.Generic;
//using Antlr.Runtime;
//using Stack = System.Collections.Generic.Stack<object>;
//using List = System.Collections.IList;
//using ArrayList = System.Collections.Generic.List<object>;

//[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.3 Nov 30, 2010 12:45:30")]
//[System.CLSCompliant(false)]
//public partial class osmosisLexer : Antlr.Runtime.Lexer
//{
//    public const int EOF=-1;
//    public const int ROOT=4;
//    public const int SPACE=5;
//    public const int WRITEXML=6;
//    public const int FILE=7;
//    public const int EQUALS=8;
//    public const int ARGUMENT=9;
//    public const int READXML=10;
//    public const int READORACLE=11;
//    public const int CONNECTIONSTRING=12;
//    public const int WRITEORACLE=13;
//    public const int READXMLCHANGE=14;
//    public const int WRITEXMLCHANGE=15;
//    public const int READORACLECHANGE=16;
//    public const int WRITEORACLECHANGE=17;
//    public const int APPLYORACLECHANGE=18;
//    public const int BB=19;
//    public const int LEFT=20;
//    public const int NUMBER=21;
//    public const int RIGHT=22;
//    public const int TOP=23;
//    public const int BOTTOM=24;
//    public const int SORT=25;
//    public const int LBRACE=26;
//    public const int RBRACE=27;

//    // delegates
//    // delegators

//    public osmosisLexer()
//    {
//        OnCreated();
//    }

//    public osmosisLexer(ICharStream input )
//        : this(input, new RecognizerSharedState())
//    {
//    }

//    public osmosisLexer(ICharStream input, RecognizerSharedState state)
//        : base(input, state)
//    {


//        OnCreated();
//    }
//    public override string GrammarFileName { get { return "C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g"; } }

//    private static readonly bool[] decisionCanBacktrack = new bool[0];

 
//    protected virtual void OnCreated() {}
//    protected virtual void EnterRule(string ruleName, int ruleIndex) {}
//    protected virtual void LeaveRule(string ruleName, int ruleIndex) {}

//    protected virtual void Enter_BB() {}
//    protected virtual void Leave_BB() {}

//    // $ANTLR start "BB"
//    [GrammarRule("BB")]
//    private void mBB()
//    {

//            try
//            {
//            int _type = BB;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:259:4: ( '--bb' | '--bounding-box' )
//            int alt1=2;
//            try { DebugEnterDecision(1, decisionCanBacktrack[1]);
//            int LA1_0 = input.LA(1);

//            if ((LA1_0=='-'))
//            {
//                int LA1_1 = input.LA(2);

//                if ((LA1_1=='-'))
//                {
//                    int LA1_2 = input.LA(3);

//                    if ((LA1_2=='b'))
//                    {
//                        int LA1_3 = input.LA(4);

//                        if ((LA1_3=='b'))
//                        {
//                            alt1=1;
//                        }
//                        else if ((LA1_3=='o'))
//                        {
//                            alt1=2;
//                        }
//                        else
//                        {
//                            NoViableAltException nvae = new NoViableAltException("", 1, 3, input);

//                            DebugRecognitionException(nvae);
//                            throw nvae;
//                        }
//                    }
//                    else
//                    {
//                        NoViableAltException nvae = new NoViableAltException("", 1, 2, input);

//                        DebugRecognitionException(nvae);
//                        throw nvae;
//                    }
//                }
//                else
//                {
//                    NoViableAltException nvae = new NoViableAltException("", 1, 1, input);

//                    DebugRecognitionException(nvae);
//                    throw nvae;
//                }
//            }
//            else
//            {
//                NoViableAltException nvae = new NoViableAltException("", 1, 0, input);

//                DebugRecognitionException(nvae);
//                throw nvae;
//            }
//            } finally { DebugExitDecision(1); }
//            switch (alt1)
//            {
//            case 1:
//                DebugEnterAlt(1);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:259:6: '--bb'
//                {
//                DebugLocation(259, 6);
//                Match("--bb"); 


//                }
//                break;
//            case 2:
//                DebugEnterAlt(2);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:259:15: '--bounding-box'
//                {
//                DebugLocation(259, 15);
//                Match("--bounding-box"); 


//                }
//                break;

//            }
//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "BB"

//    protected virtual void Enter_LEFT() {}
//    protected virtual void Leave_LEFT() {}

//    // $ANTLR start "LEFT"
//    [GrammarRule("LEFT")]
//    private void mLEFT()
//    {

//            try
//            {
//            int _type = LEFT;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:260:6: ( 'left' )
//            DebugEnterAlt(1);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:260:8: 'left'
//            {
//            DebugLocation(260, 8);
//            Match("left"); 


//            }

//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "LEFT"

//    protected virtual void Enter_RIGHT() {}
//    protected virtual void Leave_RIGHT() {}

//    // $ANTLR start "RIGHT"
//    [GrammarRule("RIGHT")]
//    private void mRIGHT()
//    {

//            try
//            {
//            int _type = RIGHT;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:261:7: ( 'right' )
//            DebugEnterAlt(1);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:261:9: 'right'
//            {
//            DebugLocation(261, 9);
//            Match("right"); 


//            }

//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "RIGHT"

//    protected virtual void Enter_TOP() {}
//    protected virtual void Leave_TOP() {}

//    // $ANTLR start "TOP"
//    [GrammarRule("TOP")]
//    private void mTOP()
//    {

//            try
//            {
//            int _type = TOP;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:262:5: ( 'top' )
//            DebugEnterAlt(1);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:262:7: 'top'
//            {
//            DebugLocation(262, 7);
//            Match("top"); 


//            }

//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "TOP"

//    protected virtual void Enter_BOTTOM() {}
//    protected virtual void Leave_BOTTOM() {}

//    // $ANTLR start "BOTTOM"
//    [GrammarRule("BOTTOM")]
//    private void mBOTTOM()
//    {

//            try
//            {
//            int _type = BOTTOM;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:263:8: ( 'bottom' )
//            DebugEnterAlt(1);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:263:10: 'bottom'
//            {
//            DebugLocation(263, 10);
//            Match("bottom"); 


//            }

//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "BOTTOM"

//    protected virtual void Enter_EQUALS() {}
//    protected virtual void Leave_EQUALS() {}

//    // $ANTLR start "EQUALS"
//    [GrammarRule("EQUALS")]
//    private void mEQUALS()
//    {

//            try
//            {
//            int _type = EQUALS;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:264:8: ( '=' )
//            DebugEnterAlt(1);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:264:10: '='
//            {
//            DebugLocation(264, 10);
//            Match('='); 

//            }

//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "EQUALS"

//    protected virtual void Enter_NUMBER() {}
//    protected virtual void Leave_NUMBER() {}

//    // $ANTLR start "NUMBER"
//    [GrammarRule("NUMBER")]
//    private void mNUMBER()
//    {

//            try
//            {
//            int _type = NUMBER;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:265:8: ( ( '0' .. '9' )+ '.' ( '0' .. '9' )+ )
//            DebugEnterAlt(1);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:265:10: ( '0' .. '9' )+ '.' ( '0' .. '9' )+
//            {
//            DebugLocation(265, 10);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:265:10: ( '0' .. '9' )+
//            int cnt2=0;
//            try { DebugEnterSubRule(2);
//            while (true)
//            {
//                int alt2=2;
//                try { DebugEnterDecision(2, decisionCanBacktrack[2]);
//                int LA2_0 = input.LA(1);

//                if (((LA2_0>='0' && LA2_0<='9')))
//                {
//                    alt2=1;
//                }


//                } finally { DebugExitDecision(2); }
//                switch (alt2)
//                {
//                case 1:
//                    DebugEnterAlt(1);
//                    // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:265:11: '0' .. '9'
//                    {
//                    DebugLocation(265, 11);
//                    MatchRange('0','9'); 

//                    }
//                    break;

//                default:
//                    if (cnt2 >= 1)
//                        goto loop2;

//                    EarlyExitException eee2 = new EarlyExitException( 2, input );
//                    DebugRecognitionException(eee2);
//                    throw eee2;
//                }
//                cnt2++;
//            }
//            loop2:
//                ;

//            } finally { DebugExitSubRule(2); }

//            DebugLocation(265, 21);
//            Match('.'); 
//            DebugLocation(265, 24);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:265:24: ( '0' .. '9' )+
//            int cnt3=0;
//            try { DebugEnterSubRule(3);
//            while (true)
//            {
//                int alt3=2;
//                try { DebugEnterDecision(3, decisionCanBacktrack[3]);
//                int LA3_0 = input.LA(1);

//                if (((LA3_0>='0' && LA3_0<='9')))
//                {
//                    alt3=1;
//                }


//                } finally { DebugExitDecision(3); }
//                switch (alt3)
//                {
//                case 1:
//                    DebugEnterAlt(1);
//                    // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:265:25: '0' .. '9'
//                    {
//                    DebugLocation(265, 25);
//                    MatchRange('0','9'); 

//                    }
//                    break;

//                default:
//                    if (cnt3 >= 1)
//                        goto loop3;

//                    EarlyExitException eee3 = new EarlyExitException( 3, input );
//                    DebugRecognitionException(eee3);
//                    throw eee3;
//                }
//                cnt3++;
//            }
//            loop3:
//                ;

//            } finally { DebugExitSubRule(3); }


//            }

//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "NUMBER"

//    protected virtual void Enter_SPACE() {}
//    protected virtual void Leave_SPACE() {}

//    // $ANTLR start "SPACE"
//    [GrammarRule("SPACE")]
//    private void mSPACE()
//    {

//            try
//            {
//            int _type = SPACE;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:266:7: ( ' ' )
//            DebugEnterAlt(1);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:266:9: ' '
//            {
//            DebugLocation(266, 9);
//            Match(' '); 

//            }

//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "SPACE"

//    protected virtual void Enter_LBRACE() {}
//    protected virtual void Leave_LBRACE() {}

//    // $ANTLR start "LBRACE"
//    [GrammarRule("LBRACE")]
//    private void mLBRACE()
//    {

//            try
//            {
//            int _type = LBRACE;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:267:8: ( '(' )
//            DebugEnterAlt(1);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:267:10: '('
//            {
//            DebugLocation(267, 10);
//            Match('('); 

//            }

//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "LBRACE"

//    protected virtual void Enter_RBRACE() {}
//    protected virtual void Leave_RBRACE() {}

//    // $ANTLR start "RBRACE"
//    [GrammarRule("RBRACE")]
//    private void mRBRACE()
//    {

//            try
//            {
//            int _type = RBRACE;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:268:8: ( ')' )
//            DebugEnterAlt(1);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:268:10: ')'
//            {
//            DebugLocation(268, 10);
//            Match(')'); 

//            }

//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "RBRACE"

//    protected virtual void Enter_FILE() {}
//    protected virtual void Leave_FILE() {}

//    // $ANTLR start "FILE"
//    [GrammarRule("FILE")]
//    private void mFILE()
//    {

//            try
//            {
//            int _type = FILE;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:269:6: ( 'file' )
//            DebugEnterAlt(1);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:269:8: 'file'
//            {
//            DebugLocation(269, 8);
//            Match("file"); 


//            }

//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "FILE"

//    protected virtual void Enter_CONNECTIONSTRING() {}
//    protected virtual void Leave_CONNECTIONSTRING() {}

//    // $ANTLR start "CONNECTIONSTRING"
//    [GrammarRule("CONNECTIONSTRING")]
//    private void mCONNECTIONSTRING()
//    {

//            try
//            {
//            int _type = CONNECTIONSTRING;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:272:2: ( 'connectionString' | 'cs' )
//            int alt4=2;
//            try { DebugEnterDecision(4, decisionCanBacktrack[4]);
//            int LA4_0 = input.LA(1);

//            if ((LA4_0=='c'))
//            {
//                int LA4_1 = input.LA(2);

//                if ((LA4_1=='o'))
//                {
//                    alt4=1;
//                }
//                else if ((LA4_1=='s'))
//                {
//                    alt4=2;
//                }
//                else
//                {
//                    NoViableAltException nvae = new NoViableAltException("", 4, 1, input);

//                    DebugRecognitionException(nvae);
//                    throw nvae;
//                }
//            }
//            else
//            {
//                NoViableAltException nvae = new NoViableAltException("", 4, 0, input);

//                DebugRecognitionException(nvae);
//                throw nvae;
//            }
//            } finally { DebugExitDecision(4); }
//            switch (alt4)
//            {
//            case 1:
//                DebugEnterAlt(1);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:272:4: 'connectionString'
//                {
//                DebugLocation(272, 4);
//                Match("connectionString"); 


//                }
//                break;
//            case 2:
//                DebugEnterAlt(2);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:272:25: 'cs'
//                {
//                DebugLocation(272, 25);
//                Match("cs"); 


//                }
//                break;

//            }
//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "CONNECTIONSTRING"

//    protected virtual void Enter_READXML() {}
//    protected virtual void Leave_READXML() {}

//    // $ANTLR start "READXML"
//    [GrammarRule("READXML")]
//    private void mREADXML()
//    {

//            try
//            {
//            int _type = READXML;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:274:2: ( '--read-xml' | '--rx' )
//            int alt5=2;
//            try { DebugEnterDecision(5, decisionCanBacktrack[5]);
//            int LA5_0 = input.LA(1);

//            if ((LA5_0=='-'))
//            {
//                int LA5_1 = input.LA(2);

//                if ((LA5_1=='-'))
//                {
//                    int LA5_2 = input.LA(3);

//                    if ((LA5_2=='r'))
//                    {
//                        int LA5_3 = input.LA(4);

//                        if ((LA5_3=='e'))
//                        {
//                            alt5=1;
//                        }
//                        else if ((LA5_3=='x'))
//                        {
//                            alt5=2;
//                        }
//                        else
//                        {
//                            NoViableAltException nvae = new NoViableAltException("", 5, 3, input);

//                            DebugRecognitionException(nvae);
//                            throw nvae;
//                        }
//                    }
//                    else
//                    {
//                        NoViableAltException nvae = new NoViableAltException("", 5, 2, input);

//                        DebugRecognitionException(nvae);
//                        throw nvae;
//                    }
//                }
//                else
//                {
//                    NoViableAltException nvae = new NoViableAltException("", 5, 1, input);

//                    DebugRecognitionException(nvae);
//                    throw nvae;
//                }
//            }
//            else
//            {
//                NoViableAltException nvae = new NoViableAltException("", 5, 0, input);

//                DebugRecognitionException(nvae);
//                throw nvae;
//            }
//            } finally { DebugExitDecision(5); }
//            switch (alt5)
//            {
//            case 1:
//                DebugEnterAlt(1);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:274:4: '--read-xml'
//                {
//                DebugLocation(274, 4);
//                Match("--read-xml"); 


//                }
//                break;
//            case 2:
//                DebugEnterAlt(2);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:274:19: '--rx'
//                {
//                DebugLocation(274, 19);
//                Match("--rx"); 


//                }
//                break;

//            }
//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "READXML"

//    protected virtual void Enter_WRITEXML() {}
//    protected virtual void Leave_WRITEXML() {}

//    // $ANTLR start "WRITEXML"
//    [GrammarRule("WRITEXML")]
//    private void mWRITEXML()
//    {

//            try
//            {
//            int _type = WRITEXML;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:276:2: ( '--write-xml' | '--wx' )
//            int alt6=2;
//            try { DebugEnterDecision(6, decisionCanBacktrack[6]);
//            int LA6_0 = input.LA(1);

//            if ((LA6_0=='-'))
//            {
//                int LA6_1 = input.LA(2);

//                if ((LA6_1=='-'))
//                {
//                    int LA6_2 = input.LA(3);

//                    if ((LA6_2=='w'))
//                    {
//                        int LA6_3 = input.LA(4);

//                        if ((LA6_3=='r'))
//                        {
//                            alt6=1;
//                        }
//                        else if ((LA6_3=='x'))
//                        {
//                            alt6=2;
//                        }
//                        else
//                        {
//                            NoViableAltException nvae = new NoViableAltException("", 6, 3, input);

//                            DebugRecognitionException(nvae);
//                            throw nvae;
//                        }
//                    }
//                    else
//                    {
//                        NoViableAltException nvae = new NoViableAltException("", 6, 2, input);

//                        DebugRecognitionException(nvae);
//                        throw nvae;
//                    }
//                }
//                else
//                {
//                    NoViableAltException nvae = new NoViableAltException("", 6, 1, input);

//                    DebugRecognitionException(nvae);
//                    throw nvae;
//                }
//            }
//            else
//            {
//                NoViableAltException nvae = new NoViableAltException("", 6, 0, input);

//                DebugRecognitionException(nvae);
//                throw nvae;
//            }
//            } finally { DebugExitDecision(6); }
//            switch (alt6)
//            {
//            case 1:
//                DebugEnterAlt(1);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:276:4: '--write-xml'
//                {
//                DebugLocation(276, 4);
//                Match("--write-xml"); 


//                }
//                break;
//            case 2:
//                DebugEnterAlt(2);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:276:20: '--wx'
//                {
//                DebugLocation(276, 20);
//                Match("--wx"); 


//                }
//                break;

//            }
//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "WRITEXML"

//    protected virtual void Enter_READXMLCHANGE() {}
//    protected virtual void Leave_READXMLCHANGE() {}

//    // $ANTLR start "READXMLCHANGE"
//    [GrammarRule("READXMLCHANGE")]
//    private void mREADXMLCHANGE()
//    {

//            try
//            {
//            int _type = READXMLCHANGE;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:278:2: ( '--read-xml-change' | '--rxc' )
//            int alt7=2;
//            try { DebugEnterDecision(7, decisionCanBacktrack[7]);
//            int LA7_0 = input.LA(1);

//            if ((LA7_0=='-'))
//            {
//                int LA7_1 = input.LA(2);

//                if ((LA7_1=='-'))
//                {
//                    int LA7_2 = input.LA(3);

//                    if ((LA7_2=='r'))
//                    {
//                        int LA7_3 = input.LA(4);

//                        if ((LA7_3=='e'))
//                        {
//                            alt7=1;
//                        }
//                        else if ((LA7_3=='x'))
//                        {
//                            alt7=2;
//                        }
//                        else
//                        {
//                            NoViableAltException nvae = new NoViableAltException("", 7, 3, input);

//                            DebugRecognitionException(nvae);
//                            throw nvae;
//                        }
//                    }
//                    else
//                    {
//                        NoViableAltException nvae = new NoViableAltException("", 7, 2, input);

//                        DebugRecognitionException(nvae);
//                        throw nvae;
//                    }
//                }
//                else
//                {
//                    NoViableAltException nvae = new NoViableAltException("", 7, 1, input);

//                    DebugRecognitionException(nvae);
//                    throw nvae;
//                }
//            }
//            else
//            {
//                NoViableAltException nvae = new NoViableAltException("", 7, 0, input);

//                DebugRecognitionException(nvae);
//                throw nvae;
//            }
//            } finally { DebugExitDecision(7); }
//            switch (alt7)
//            {
//            case 1:
//                DebugEnterAlt(1);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:278:4: '--read-xml-change'
//                {
//                DebugLocation(278, 4);
//                Match("--read-xml-change"); 


//                }
//                break;
//            case 2:
//                DebugEnterAlt(2);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:278:26: '--rxc'
//                {
//                DebugLocation(278, 26);
//                Match("--rxc"); 


//                }
//                break;

//            }
//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "READXMLCHANGE"

//    protected virtual void Enter_WRITEXMLCHANGE() {}
//    protected virtual void Leave_WRITEXMLCHANGE() {}

//    // $ANTLR start "WRITEXMLCHANGE"
//    [GrammarRule("WRITEXMLCHANGE")]
//    private void mWRITEXMLCHANGE()
//    {

//            try
//            {
//            int _type = WRITEXMLCHANGE;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:280:2: ( '--write-xml-change' | '--wxc' )
//            int alt8=2;
//            try { DebugEnterDecision(8, decisionCanBacktrack[8]);
//            int LA8_0 = input.LA(1);

//            if ((LA8_0=='-'))
//            {
//                int LA8_1 = input.LA(2);

//                if ((LA8_1=='-'))
//                {
//                    int LA8_2 = input.LA(3);

//                    if ((LA8_2=='w'))
//                    {
//                        int LA8_3 = input.LA(4);

//                        if ((LA8_3=='r'))
//                        {
//                            alt8=1;
//                        }
//                        else if ((LA8_3=='x'))
//                        {
//                            alt8=2;
//                        }
//                        else
//                        {
//                            NoViableAltException nvae = new NoViableAltException("", 8, 3, input);

//                            DebugRecognitionException(nvae);
//                            throw nvae;
//                        }
//                    }
//                    else
//                    {
//                        NoViableAltException nvae = new NoViableAltException("", 8, 2, input);

//                        DebugRecognitionException(nvae);
//                        throw nvae;
//                    }
//                }
//                else
//                {
//                    NoViableAltException nvae = new NoViableAltException("", 8, 1, input);

//                    DebugRecognitionException(nvae);
//                    throw nvae;
//                }
//            }
//            else
//            {
//                NoViableAltException nvae = new NoViableAltException("", 8, 0, input);

//                DebugRecognitionException(nvae);
//                throw nvae;
//            }
//            } finally { DebugExitDecision(8); }
//            switch (alt8)
//            {
//            case 1:
//                DebugEnterAlt(1);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:280:4: '--write-xml-change'
//                {
//                DebugLocation(280, 4);
//                Match("--write-xml-change"); 


//                }
//                break;
//            case 2:
//                DebugEnterAlt(2);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:280:27: '--wxc'
//                {
//                DebugLocation(280, 27);
//                Match("--wxc"); 


//                }
//                break;

//            }
//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "WRITEXMLCHANGE"

//    protected virtual void Enter_READORACLE() {}
//    protected virtual void Leave_READORACLE() {}

//    // $ANTLR start "READORACLE"
//    [GrammarRule("READORACLE")]
//    private void mREADORACLE()
//    {

//            try
//            {
//            int _type = READORACLE;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:282:2: ( '--read-oracle' | '--ro' )
//            int alt9=2;
//            try { DebugEnterDecision(9, decisionCanBacktrack[9]);
//            int LA9_0 = input.LA(1);

//            if ((LA9_0=='-'))
//            {
//                int LA9_1 = input.LA(2);

//                if ((LA9_1=='-'))
//                {
//                    int LA9_2 = input.LA(3);

//                    if ((LA9_2=='r'))
//                    {
//                        int LA9_3 = input.LA(4);

//                        if ((LA9_3=='e'))
//                        {
//                            alt9=1;
//                        }
//                        else if ((LA9_3=='o'))
//                        {
//                            alt9=2;
//                        }
//                        else
//                        {
//                            NoViableAltException nvae = new NoViableAltException("", 9, 3, input);

//                            DebugRecognitionException(nvae);
//                            throw nvae;
//                        }
//                    }
//                    else
//                    {
//                        NoViableAltException nvae = new NoViableAltException("", 9, 2, input);

//                        DebugRecognitionException(nvae);
//                        throw nvae;
//                    }
//                }
//                else
//                {
//                    NoViableAltException nvae = new NoViableAltException("", 9, 1, input);

//                    DebugRecognitionException(nvae);
//                    throw nvae;
//                }
//            }
//            else
//            {
//                NoViableAltException nvae = new NoViableAltException("", 9, 0, input);

//                DebugRecognitionException(nvae);
//                throw nvae;
//            }
//            } finally { DebugExitDecision(9); }
//            switch (alt9)
//            {
//            case 1:
//                DebugEnterAlt(1);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:282:4: '--read-oracle'
//                {
//                DebugLocation(282, 4);
//                Match("--read-oracle"); 


//                }
//                break;
//            case 2:
//                DebugEnterAlt(2);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:282:22: '--ro'
//                {
//                DebugLocation(282, 22);
//                Match("--ro"); 


//                }
//                break;

//            }
//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "READORACLE"

//    protected virtual void Enter_WRITEORACLE() {}
//    protected virtual void Leave_WRITEORACLE() {}

//    // $ANTLR start "WRITEORACLE"
//    [GrammarRule("WRITEORACLE")]
//    private void mWRITEORACLE()
//    {

//            try
//            {
//            int _type = WRITEORACLE;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:284:2: ( '--write-oracle' | '--wo' )
//            int alt10=2;
//            try { DebugEnterDecision(10, decisionCanBacktrack[10]);
//            int LA10_0 = input.LA(1);

//            if ((LA10_0=='-'))
//            {
//                int LA10_1 = input.LA(2);

//                if ((LA10_1=='-'))
//                {
//                    int LA10_2 = input.LA(3);

//                    if ((LA10_2=='w'))
//                    {
//                        int LA10_3 = input.LA(4);

//                        if ((LA10_3=='r'))
//                        {
//                            alt10=1;
//                        }
//                        else if ((LA10_3=='o'))
//                        {
//                            alt10=2;
//                        }
//                        else
//                        {
//                            NoViableAltException nvae = new NoViableAltException("", 10, 3, input);

//                            DebugRecognitionException(nvae);
//                            throw nvae;
//                        }
//                    }
//                    else
//                    {
//                        NoViableAltException nvae = new NoViableAltException("", 10, 2, input);

//                        DebugRecognitionException(nvae);
//                        throw nvae;
//                    }
//                }
//                else
//                {
//                    NoViableAltException nvae = new NoViableAltException("", 10, 1, input);

//                    DebugRecognitionException(nvae);
//                    throw nvae;
//                }
//            }
//            else
//            {
//                NoViableAltException nvae = new NoViableAltException("", 10, 0, input);

//                DebugRecognitionException(nvae);
//                throw nvae;
//            }
//            } finally { DebugExitDecision(10); }
//            switch (alt10)
//            {
//            case 1:
//                DebugEnterAlt(1);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:284:4: '--write-oracle'
//                {
//                DebugLocation(284, 4);
//                Match("--write-oracle"); 


//                }
//                break;
//            case 2:
//                DebugEnterAlt(2);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:284:23: '--wo'
//                {
//                DebugLocation(284, 23);
//                Match("--wo"); 


//                }
//                break;

//            }
//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "WRITEORACLE"

//    protected virtual void Enter_READORACLECHANGE() {}
//    protected virtual void Leave_READORACLECHANGE() {}

//    // $ANTLR start "READORACLECHANGE"
//    [GrammarRule("READORACLECHANGE")]
//    private void mREADORACLECHANGE()
//    {

//            try
//            {
//            int _type = READORACLECHANGE;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:286:2: ( '--read-oracle-change' | '--roc' )
//            int alt11=2;
//            try { DebugEnterDecision(11, decisionCanBacktrack[11]);
//            int LA11_0 = input.LA(1);

//            if ((LA11_0=='-'))
//            {
//                int LA11_1 = input.LA(2);

//                if ((LA11_1=='-'))
//                {
//                    int LA11_2 = input.LA(3);

//                    if ((LA11_2=='r'))
//                    {
//                        int LA11_3 = input.LA(4);

//                        if ((LA11_3=='e'))
//                        {
//                            alt11=1;
//                        }
//                        else if ((LA11_3=='o'))
//                        {
//                            alt11=2;
//                        }
//                        else
//                        {
//                            NoViableAltException nvae = new NoViableAltException("", 11, 3, input);

//                            DebugRecognitionException(nvae);
//                            throw nvae;
//                        }
//                    }
//                    else
//                    {
//                        NoViableAltException nvae = new NoViableAltException("", 11, 2, input);

//                        DebugRecognitionException(nvae);
//                        throw nvae;
//                    }
//                }
//                else
//                {
//                    NoViableAltException nvae = new NoViableAltException("", 11, 1, input);

//                    DebugRecognitionException(nvae);
//                    throw nvae;
//                }
//            }
//            else
//            {
//                NoViableAltException nvae = new NoViableAltException("", 11, 0, input);

//                DebugRecognitionException(nvae);
//                throw nvae;
//            }
//            } finally { DebugExitDecision(11); }
//            switch (alt11)
//            {
//            case 1:
//                DebugEnterAlt(1);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:286:4: '--read-oracle-change'
//                {
//                DebugLocation(286, 4);
//                Match("--read-oracle-change"); 


//                }
//                break;
//            case 2:
//                DebugEnterAlt(2);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:286:29: '--roc'
//                {
//                DebugLocation(286, 29);
//                Match("--roc"); 


//                }
//                break;

//            }
//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "READORACLECHANGE"

//    protected virtual void Enter_WRITEORACLECHANGE() {}
//    protected virtual void Leave_WRITEORACLECHANGE() {}

//    // $ANTLR start "WRITEORACLECHANGE"
//    [GrammarRule("WRITEORACLECHANGE")]
//    private void mWRITEORACLECHANGE()
//    {

//            try
//            {
//            int _type = WRITEORACLECHANGE;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:288:2: ( '--write-oracle-change' | '--woc' )
//            int alt12=2;
//            try { DebugEnterDecision(12, decisionCanBacktrack[12]);
//            int LA12_0 = input.LA(1);

//            if ((LA12_0=='-'))
//            {
//                int LA12_1 = input.LA(2);

//                if ((LA12_1=='-'))
//                {
//                    int LA12_2 = input.LA(3);

//                    if ((LA12_2=='w'))
//                    {
//                        int LA12_3 = input.LA(4);

//                        if ((LA12_3=='r'))
//                        {
//                            alt12=1;
//                        }
//                        else if ((LA12_3=='o'))
//                        {
//                            alt12=2;
//                        }
//                        else
//                        {
//                            NoViableAltException nvae = new NoViableAltException("", 12, 3, input);

//                            DebugRecognitionException(nvae);
//                            throw nvae;
//                        }
//                    }
//                    else
//                    {
//                        NoViableAltException nvae = new NoViableAltException("", 12, 2, input);

//                        DebugRecognitionException(nvae);
//                        throw nvae;
//                    }
//                }
//                else
//                {
//                    NoViableAltException nvae = new NoViableAltException("", 12, 1, input);

//                    DebugRecognitionException(nvae);
//                    throw nvae;
//                }
//            }
//            else
//            {
//                NoViableAltException nvae = new NoViableAltException("", 12, 0, input);

//                DebugRecognitionException(nvae);
//                throw nvae;
//            }
//            } finally { DebugExitDecision(12); }
//            switch (alt12)
//            {
//            case 1:
//                DebugEnterAlt(1);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:288:4: '--write-oracle-change'
//                {
//                DebugLocation(288, 4);
//                Match("--write-oracle-change"); 


//                }
//                break;
//            case 2:
//                DebugEnterAlt(2);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:288:30: '--woc'
//                {
//                DebugLocation(288, 30);
//                Match("--woc"); 


//                }
//                break;

//            }
//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "WRITEORACLECHANGE"

//    protected virtual void Enter_APPLYORACLECHANGE() {}
//    protected virtual void Leave_APPLYORACLECHANGE() {}

//    // $ANTLR start "APPLYORACLECHANGE"
//    [GrammarRule("APPLYORACLECHANGE")]
//    private void mAPPLYORACLECHANGE()
//    {

//            try
//            {
//            int _type = APPLYORACLECHANGE;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:290:2: ( '--apply-oracle-change' | '--aoc' )
//            int alt13=2;
//            try { DebugEnterDecision(13, decisionCanBacktrack[13]);
//            int LA13_0 = input.LA(1);

//            if ((LA13_0=='-'))
//            {
//                int LA13_1 = input.LA(2);

//                if ((LA13_1=='-'))
//                {
//                    int LA13_2 = input.LA(3);

//                    if ((LA13_2=='a'))
//                    {
//                        int LA13_3 = input.LA(4);

//                        if ((LA13_3=='p'))
//                        {
//                            alt13=1;
//                        }
//                        else if ((LA13_3=='o'))
//                        {
//                            alt13=2;
//                        }
//                        else
//                        {
//                            NoViableAltException nvae = new NoViableAltException("", 13, 3, input);

//                            DebugRecognitionException(nvae);
//                            throw nvae;
//                        }
//                    }
//                    else
//                    {
//                        NoViableAltException nvae = new NoViableAltException("", 13, 2, input);

//                        DebugRecognitionException(nvae);
//                        throw nvae;
//                    }
//                }
//                else
//                {
//                    NoViableAltException nvae = new NoViableAltException("", 13, 1, input);

//                    DebugRecognitionException(nvae);
//                    throw nvae;
//                }
//            }
//            else
//            {
//                NoViableAltException nvae = new NoViableAltException("", 13, 0, input);

//                DebugRecognitionException(nvae);
//                throw nvae;
//            }
//            } finally { DebugExitDecision(13); }
//            switch (alt13)
//            {
//            case 1:
//                DebugEnterAlt(1);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:290:4: '--apply-oracle-change'
//                {
//                DebugLocation(290, 4);
//                Match("--apply-oracle-change"); 


//                }
//                break;
//            case 2:
//                DebugEnterAlt(2);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:290:30: '--aoc'
//                {
//                DebugLocation(290, 30);
//                Match("--aoc"); 


//                }
//                break;

//            }
//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "APPLYORACLECHANGE"

//    protected virtual void Enter_SORT() {}
//    protected virtual void Leave_SORT() {}

//    // $ANTLR start "SORT"
//    [GrammarRule("SORT")]
//    private void mSORT()
//    {

//            try
//            {
//            int _type = SORT;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:291:6: ( '--sort' | '-s' )
//            int alt14=2;
//            try { DebugEnterDecision(14, decisionCanBacktrack[14]);
//            int LA14_0 = input.LA(1);

//            if ((LA14_0=='-'))
//            {
//                int LA14_1 = input.LA(2);

//                if ((LA14_1=='-'))
//                {
//                    alt14=1;
//                }
//                else if ((LA14_1=='s'))
//                {
//                    alt14=2;
//                }
//                else
//                {
//                    NoViableAltException nvae = new NoViableAltException("", 14, 1, input);

//                    DebugRecognitionException(nvae);
//                    throw nvae;
//                }
//            }
//            else
//            {
//                NoViableAltException nvae = new NoViableAltException("", 14, 0, input);

//                DebugRecognitionException(nvae);
//                throw nvae;
//            }
//            } finally { DebugExitDecision(14); }
//            switch (alt14)
//            {
//            case 1:
//                DebugEnterAlt(1);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:291:8: '--sort'
//                {
//                DebugLocation(291, 8);
//                Match("--sort"); 


//                }
//                break;
//            case 2:
//                DebugEnterAlt(2);
//                // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:291:19: '-s'
//                {
//                DebugLocation(291, 19);
//                Match("-s"); 


//                }
//                break;

//            }
//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "SORT"

//    protected virtual void Enter_ARGUMENT() {}
//    protected virtual void Leave_ARGUMENT() {}

//    // $ANTLR start "ARGUMENT"
//    [GrammarRule("ARGUMENT")]
//    private void mARGUMENT()
//    {

//            try
//            {
//            int _type = ARGUMENT;
//            int _channel = DefaultTokenChannel;
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:293:3: ( '\"' (~ '\"' )+ '\"' )
//            DebugEnterAlt(1);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:293:5: '\"' (~ '\"' )+ '\"'
//            {
//            DebugLocation(293, 5);
//            Match('\"'); 
//            DebugLocation(293, 8);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:293:8: (~ '\"' )+
//            int cnt15=0;
//            try { DebugEnterSubRule(15);
//            while (true)
//            {
//                int alt15=2;
//                try { DebugEnterDecision(15, decisionCanBacktrack[15]);
//                int LA15_0 = input.LA(1);

//                if (((LA15_0>='\u0000' && LA15_0<='!')||(LA15_0>='#' && LA15_0<='\uFFFF')))
//                {
//                    alt15=1;
//                }


//                } finally { DebugExitDecision(15); }
//                switch (alt15)
//                {
//                case 1:
//                    DebugEnterAlt(1);
//                    // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:293:9: ~ '\"'
//                    {
//                    DebugLocation(293, 9);
//                    if ((input.LA(1)>='\u0000' && input.LA(1)<='!')||(input.LA(1)>='#' && input.LA(1)<='\uFFFF'))
//                    {
//                        input.Consume();

//                    }
//                    else
//                    {
//                        MismatchedSetException mse = new MismatchedSetException(null,input);
//                        DebugRecognitionException(mse);
//                        Recover(mse);
//                        throw mse;}


//                    }
//                    break;

//                default:
//                    if (cnt15 >= 1)
//                        goto loop15;

//                    EarlyExitException eee15 = new EarlyExitException( 15, input );
//                    DebugRecognitionException(eee15);
//                    throw eee15;
//                }
//                cnt15++;
//            }
//            loop15:
//                ;

//            } finally { DebugExitSubRule(15); }

//            DebugLocation(293, 15);
//            Match('\"'); 

//            }

//            state.type = _type;
//            state.channel = _channel;
//        }
//        finally
//        {
//        }
//    }
//    // $ANTLR end "ARGUMENT"

//    public override void mTokens()
//    {
//        // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:8: ( BB | LEFT | RIGHT | TOP | BOTTOM | EQUALS | NUMBER | SPACE | LBRACE | RBRACE | FILE | CONNECTIONSTRING | READXML | WRITEXML | READXMLCHANGE | WRITEXMLCHANGE | READORACLE | WRITEORACLE | READORACLECHANGE | WRITEORACLECHANGE | APPLYORACLECHANGE | SORT | ARGUMENT )
//        int alt16=23;
//        try { DebugEnterDecision(16, decisionCanBacktrack[16]);
//        try
//        {
//            alt16 = dfa16.Predict(input);
//        }
//        catch (NoViableAltException nvae)
//        {
//            DebugRecognitionException(nvae);
//            throw;
//        }
//        } finally { DebugExitDecision(16); }
//        switch (alt16)
//        {
//        case 1:
//            DebugEnterAlt(1);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:10: BB
//            {
//            DebugLocation(1, 10);
//            mBB(); 

//            }
//            break;
//        case 2:
//            DebugEnterAlt(2);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:13: LEFT
//            {
//            DebugLocation(1, 13);
//            mLEFT(); 

//            }
//            break;
//        case 3:
//            DebugEnterAlt(3);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:18: RIGHT
//            {
//            DebugLocation(1, 18);
//            mRIGHT(); 

//            }
//            break;
//        case 4:
//            DebugEnterAlt(4);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:24: TOP
//            {
//            DebugLocation(1, 24);
//            mTOP(); 

//            }
//            break;
//        case 5:
//            DebugEnterAlt(5);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:28: BOTTOM
//            {
//            DebugLocation(1, 28);
//            mBOTTOM(); 

//            }
//            break;
//        case 6:
//            DebugEnterAlt(6);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:35: EQUALS
//            {
//            DebugLocation(1, 35);
//            mEQUALS(); 

//            }
//            break;
//        case 7:
//            DebugEnterAlt(7);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:42: NUMBER
//            {
//            DebugLocation(1, 42);
//            mNUMBER(); 

//            }
//            break;
//        case 8:
//            DebugEnterAlt(8);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:49: SPACE
//            {
//            DebugLocation(1, 49);
//            mSPACE(); 

//            }
//            break;
//        case 9:
//            DebugEnterAlt(9);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:55: LBRACE
//            {
//            DebugLocation(1, 55);
//            mLBRACE(); 

//            }
//            break;
//        case 10:
//            DebugEnterAlt(10);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:62: RBRACE
//            {
//            DebugLocation(1, 62);
//            mRBRACE(); 

//            }
//            break;
//        case 11:
//            DebugEnterAlt(11);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:69: FILE
//            {
//            DebugLocation(1, 69);
//            mFILE(); 

//            }
//            break;
//        case 12:
//            DebugEnterAlt(12);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:74: CONNECTIONSTRING
//            {
//            DebugLocation(1, 74);
//            mCONNECTIONSTRING(); 

//            }
//            break;
//        case 13:
//            DebugEnterAlt(13);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:91: READXML
//            {
//            DebugLocation(1, 91);
//            mREADXML(); 

//            }
//            break;
//        case 14:
//            DebugEnterAlt(14);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:99: WRITEXML
//            {
//            DebugLocation(1, 99);
//            mWRITEXML(); 

//            }
//            break;
//        case 15:
//            DebugEnterAlt(15);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:108: READXMLCHANGE
//            {
//            DebugLocation(1, 108);
//            mREADXMLCHANGE(); 

//            }
//            break;
//        case 16:
//            DebugEnterAlt(16);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:122: WRITEXMLCHANGE
//            {
//            DebugLocation(1, 122);
//            mWRITEXMLCHANGE(); 

//            }
//            break;
//        case 17:
//            DebugEnterAlt(17);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:137: READORACLE
//            {
//            DebugLocation(1, 137);
//            mREADORACLE(); 

//            }
//            break;
//        case 18:
//            DebugEnterAlt(18);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:148: WRITEORACLE
//            {
//            DebugLocation(1, 148);
//            mWRITEORACLE(); 

//            }
//            break;
//        case 19:
//            DebugEnterAlt(19);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:160: READORACLECHANGE
//            {
//            DebugLocation(1, 160);
//            mREADORACLECHANGE(); 

//            }
//            break;
//        case 20:
//            DebugEnterAlt(20);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:177: WRITEORACLECHANGE
//            {
//            DebugLocation(1, 177);
//            mWRITEORACLECHANGE(); 

//            }
//            break;
//        case 21:
//            DebugEnterAlt(21);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:195: APPLYORACLECHANGE
//            {
//            DebugLocation(1, 195);
//            mAPPLYORACLECHANGE(); 

//            }
//            break;
//        case 22:
//            DebugEnterAlt(22);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:213: SORT
//            {
//            DebugLocation(1, 213);
//            mSORT(); 

//            }
//            break;
//        case 23:
//            DebugEnterAlt(23);
//            // C:\\PRIVATE\\Projects\\OsmSharp\\svn\\OsmSharp\\Osm.DataProcessor.Main\\ArgumentsParser\\osmosis.g:1:218: ARGUMENT
//            {
//            DebugLocation(1, 218);
//            mARGUMENT(); 

//            }
//            break;

//        }

//    }


//    #region DFA
//    DFA16 dfa16;

//    protected override void InitDFAs()
//    {
//        base.InitDFAs();
//        dfa16 = new DFA16(this);
//    }

//    private class DFA16 : DFA
//    {
//        private const string DFA16_eotS =
//            "\x15\uffff\x01\x1c\x01\x1e\x01\uffff\x01\x21\x01\x23\x15\uffff\x01\x1c"+
//            "\x04\uffff\x01\x21\x03\uffff\x01\x1e\x01\uffff\x01\x23";
//        private const string DFA16_eofS =
//            "\x3b\uffff";
//        private const string DFA16_minS =
//            "\x01\x20\x01\x2d\x0c\uffff\x01\x61\x02\uffff\x01\x65\x01\x6f\x01\uffff"+
//            "\x01\x61\x02\x63\x01\x69\x02\x63\x01\x64\x04\uffff\x01\x74\x04\uffff"+
//            "\x01\x2d\x01\x65\x01\x6f\x01\x2d\x01\x6d\x01\x72\x01\x6f\x01\x6c\x01"+
//            "\x61\x01\x6d\x01\x72\x01\x2d\x01\x63\x01\x6c\x01\x61\x01\x6c\x01\x2d"+
//            "\x01\x63\x01\x65\x01\x6c\x01\x2d\x01\x65\x01\x2d";
//        private const string DFA16_maxS =
//            "\x01\x74\x01\x73\x0c\uffff\x01\x77\x02\uffff\x02\x78\x01\uffff\x01\x61"+
//            "\x02\x63\x01\x69\x02\x63\x01\x64\x04\uffff\x01\x74\x04\uffff\x01\x2d"+
//            "\x01\x65\x01\x78\x01\x2d\x01\x6d\x01\x72\x01\x78\x01\x6c\x01\x61\x01"+
//            "\x6d\x01\x72\x01\x2d\x01\x63\x01\x6c\x01\x61\x01\x6c\x01\x2d\x01\x63"+
//            "\x01\x65\x01\x6c\x01\x2d\x01\x65\x01\x2d";
//        private const string DFA16_acceptS =
//            "\x02\uffff\x01\x02\x01\x03\x01\x04\x01\x05\x01\x06\x01\x07\x01\x08\x01"+
//            "\x09\x01\x0a\x01\x0b\x01\x0c\x01\x17\x01\uffff\x01\x16\x01\x01\x02\uffff"+
//            "\x01\x15\x07\uffff\x01\x0f\x01\x0d\x01\x13\x01\x11\x01\uffff\x01\x10"+
//            "\x01\x0e\x01\x14\x01\x12\x17\uffff";
//        private const string DFA16_specialS =
//            "\x3b\uffff}>";
//        private static readonly string[] DFA16_transitionS =
//            {
//                "\x01\x08\x01\uffff\x01\x0d\x05\uffff\x01\x09\x01\x0a\x03\uffff\x01"+
//                "\x01\x02\uffff\x0a\x07\x03\uffff\x01\x06\x24\uffff\x01\x05\x01\x0c\x02"+
//                "\uffff\x01\x0b\x05\uffff\x01\x02\x05\uffff\x01\x03\x01\uffff\x01\x04",
//                "\x01\x0e\x45\uffff\x01\x0f",
//                "",
//                "",
//                "",
//                "",
//                "",
//                "",
//                "",
//                "",
//                "",
//                "",
//                "",
//                "",
//                "\x01\x13\x01\x10\x0f\uffff\x01\x11\x01\x0f\x03\uffff\x01\x12",
//                "",
//                "",
//                "\x01\x14\x09\uffff\x01\x16\x08\uffff\x01\x15",
//                "\x01\x19\x02\uffff\x01\x17\x05\uffff\x01\x18",
//                "",
//                "\x01\x1a",
//                "\x01\x1b",
//                "\x01\x1d",
//                "\x01\x1f",
//                "\x01\x20",
//                "\x01\x22",
//                "\x01\x24",
//                "",
//                "",
//                "",
//                "",
//                "\x01\x25",
//                "",
//                "",
//                "",
//                "",
//                "\x01\x26",
//                "\x01\x27",
//                "\x01\x29\x08\uffff\x01\x28",
//                "\x01\x2a",
//                "\x01\x2b",
//                "\x01\x2c",
//                "\x01\x2e\x08\uffff\x01\x2d",
//                "\x01\x2f",
//                "\x01\x30",
//                "\x01\x31",
//                "\x01\x32",
//                "\x01\x1b",
//                "\x01\x33",
//                "\x01\x34",
//                "\x01\x35",
//                "\x01\x36",
//                "\x01\x20",
//                "\x01\x37",
//                "\x01\x38",
//                "\x01\x39",
//                "\x01\x1d",
//                "\x01\x3a",
//                "\x01\x22"
//            };

//        private static readonly short[] DFA16_eot = DFA.UnpackEncodedString(DFA16_eotS);
//        private static readonly short[] DFA16_eof = DFA.UnpackEncodedString(DFA16_eofS);
//        private static readonly char[] DFA16_min = DFA.UnpackEncodedStringToUnsignedChars(DFA16_minS);
//        private static readonly char[] DFA16_max = DFA.UnpackEncodedStringToUnsignedChars(DFA16_maxS);
//        private static readonly short[] DFA16_accept = DFA.UnpackEncodedString(DFA16_acceptS);
//        private static readonly short[] DFA16_special = DFA.UnpackEncodedString(DFA16_specialS);
//        private static readonly short[][] DFA16_transition;

//        static DFA16()
//        {
//            int numStates = DFA16_transitionS.Length;
//            DFA16_transition = new short[numStates][];
//            for ( int i=0; i < numStates; i++ )
//            {
//                DFA16_transition[i] = DFA.UnpackEncodedString(DFA16_transitionS[i]);
//            }
//        }

//        public DFA16( BaseRecognizer recognizer )
//        {
//            this.recognizer = recognizer;
//            this.decisionNumber = 16;
//            this.eot = DFA16_eot;
//            this.eof = DFA16_eof;
//            this.min = DFA16_min;
//            this.max = DFA16_max;
//            this.accept = DFA16_accept;
//            this.special = DFA16_special;
//            this.transition = DFA16_transition;
//        }

//        public override string Description { get { return "1:1: Tokens : ( BB | LEFT | RIGHT | TOP | BOTTOM | EQUALS | NUMBER | SPACE | LBRACE | RBRACE | FILE | CONNECTIONSTRING | READXML | WRITEXML | READXMLCHANGE | WRITEXMLCHANGE | READORACLE | WRITEORACLE | READORACLECHANGE | WRITEORACLECHANGE | APPLYORACLECHANGE | SORT | ARGUMENT );"; } }

//        public override void Error(NoViableAltException nvae)
//        {
//            DebugRecognitionException(nvae);
//        }
//    }

 
//    #endregion

//}
