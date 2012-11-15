grammar osmosis;

options
{
    language=CSharp2;
    output=AST;
}

tokens 
{
  ROOT;
}

@header {
using OsmSharp.Osm.DataProcessor.Core;
using OsmSharp.Osm.DataProcessor.Core.ChangeSets;
using OsmSharp.Osm.DataProcessor.Main;
}

public parse returns [object target]
	: exp EOF!
	{
		$target=$exp.target;
	};
	
exp returns [object target]
	: (processexp 
	{
		$target = $processexp.target;
	}
	| changeprocessexp
	{
		$target = null;
	});

// processing osm files.
processexp1 returns [DataProcessorTarget target,DataProcessorFilter filter]
	:processop SPACE! 
	(outputexp
	{
		$filter = $processop.filter;
		$target = $outputexp.target;
	}
	| processexp2
	{
		// recursive filtering happens here!
		$filter = ArgumentsParser.LinkFilterToSource($processop.filter,$processexp2.filter);
		$target = $processexp2.target;
	});
processexp2 returns [DataProcessorTarget target,DataProcessorFilter filter]
	:	processexp1
	{
		$filter=$processexp1.filter;
		$target=$processexp1.target;
	};
processexp returns [DataProcessorTarget target]
	:	(inputexp SPACE! 
		(outputexp
		{
			ArgumentsParser.LinkSourceToTarget($inputexp.source,$outputexp.target);
			$target = $outputexp.target;
		}
		| processexp1
		{
			if($processexp1.filter == null)
			{
				// there is not filter so just link the source.				
				ArgumentsParser.LinkSourceToTarget($inputexp.source,$outputexp.target);
			}
			else
			{
				// link the filter to the source.	
				// link the filter as the source to the target.
				ArgumentsParser.LinkSourceToTarget(
					ArgumentsParser.LinkFilterToSource($processexp1.filter,$inputexp.source)
						,$processexp1.target);
			}
			$target = $processexp1.target;
		}));	
inputexp returns [DataProcessorSource source]
	:	(xmlfilein
		{
			$source = $xmlfilein.source;
		}
		| dbin
		{
			$source = $dbin.source;
		}
		);
outputexp returns [DataProcessorTarget target]
	:	(xmlfileout
		{
			$target = $xmlfileout.target;
		}
		| dbout
		{
			$target = $dbout.target;
		});	
processop returns [DataProcessorFilter filter]
		:(bbop
		{
			$filter = $bbop.filter;
		} 
		| sortop
		{
			$filter = $sortop.filter;
		});
xmlfileout returns [DataProcessorTarget target]
	:	WRITEXML^ SPACE! FILE EQUALS! ARGUMENT
		{
			$target = ArgumentsParser.CreateXmlTarget($ARGUMENT.text);
		};
xmlfilein returns [DataProcessorSource source]
	:	READXML^ SPACE! FILE EQUALS! ARGUMENT
		{
			$source = ArgumentsParser.CreateXmlSource($ARGUMENT.text);
		};
dbin returns [DataProcessorSource source]	
	:	(READORACLE^) SPACE! CONNECTIONSTRING EQUALS! ARGUMENT
		{
			$source = ArgumentsParser.CreateOracleSource($ARGUMENT.text);
		};
dbout returns [DataProcessorTarget target]
	:	(WRITEORACLE^) SPACE! CONNECTIONSTRING EQUALS! ARGUMENT
		{
			$target = ArgumentsParser.CreateOracleTarget($ARGUMENT.text);
		};
// merging osm files
// TODO: expand on this to allow recursion.
//mergeop:	inputexp SPACE inputexp MERGE outputexp;
// processing osm changeset files
changeprocessexp1 returns [DataProcessorChangeSetTarget target,DataProcessorChangeSetFilter filter]
	:	changeprocessop SPACE! 
		(changeoutputexp 
		{
			$filter = $changeprocessop.filter;
			$target = $changeoutputexp.target;
		}
		| changeprocessexp2
		{
			// recursive filtering happens here!
			$filter = ArgumentsParser.LinkChangeFilterToSource($changeprocessop.filter,$changeprocessexp2.filter);
			$target = $changeprocessexp2.target;
		});
changeprocessexp2 returns [DataProcessorChangeSetTarget target,DataProcessorChangeSetFilter filter]
	:	changeprocessexp1
	{
		$filter=$changeprocessexp1.filter;
		$target=$changeprocessexp1.target;
	};
changeprocessexp returns [DataProcessorChangeSetTarget target]
	:	changeinputexp SPACE! 
		(changeoutputexp 
		{
			ArgumentsParser.LinkChangeSourceToTarget($changeinputexp.source,$changeoutputexp.target);
			$target = $changeoutputexp.target;
		}
		| changeprocessexp1)
		{
			if($changeprocessexp1.filter == null)
			{
				// there is not filter so just link the source.				
				ArgumentsParser.LinkChangeSourceToTarget($changeinputexp.source,$changeoutputexp.target);
			}
			else
			{
				// link the filter to the source.	
				// link the filter as the source to the target.
				ArgumentsParser.LinkChangeSourceToTarget(
					ArgumentsParser.LinkChangeFilterToSource($changeprocessexp1.filter,$changeinputexp.source)
						,$changeprocessexp1.target);
			}
			$target = $changeprocessexp1.target;
		};	
changeinputexp returns [DataProcessorChangeSetSource source]
	:	(changexmlfilein
		{
			$source = $changexmlfilein.source;
		} 
		| changedbin
		{
			throw new NotImplementedException();
		});
changeoutputexp returns [DataProcessorChangeSetTarget target]
	:	(changexmlfileout
		{
			$target = $changexmlfileout.target;
		}  
		|changedbout
		{
			throw new NotImplementedException();
		}
		|changeapplydbout
		{
			$target = $changeapplydbout.target;
		});	
changeprocessop returns [DataProcessorChangeSetFilter filter]
	:	(changebbop
	{
		$filter = $changebbop.filter;
	});	
changexmlfilein returns [DataProcessorChangeSetSource source]
	:	READXMLCHANGE^ SPACE! FILE EQUALS! ARGUMENT	
	{
		$source = ArgumentsParser.CreateChangeXmlSource($ARGUMENT.text);
	};
changexmlfileout returns [DataProcessorChangeSetTarget target]
	:	WRITEXMLCHANGE^ SPACE! FILE EQUALS! ARGUMENT
	{
		$target = ArgumentsParser.CreateChangeXmlTarget($ARGUMENT.text);
	};
changedbin	:	(READORACLECHANGE) SPACE! CONNECTIONSTRING EQUALS! ARGUMENT;
changedbout	:	(WRITEORACLECHANGE) SPACE! CONNECTIONSTRING EQUALS! ARGUMENT;
changeapplydbout returns [DataProcessorChangeSetTarget target]
	:	(APPLYORACLECHANGE) SPACE! CONNECTIONSTRING EQUALS! ARGUMENT
	{
		$target = ArgumentsParser.CreateChangeApplyOracleTarget($ARGUMENT.text);
	};

// operators
// bb operator		
changebbop returns [DataProcessorChangeSetFilter filter]
	:	BB^ SPACE! bbleft SPACE! bbright SPACE! bbtop SPACE! bbbottom
	{
		$filter = ArgumentsParser.CreateChangeBoundingBoxFilter($bbleft.value,$bbright.value,$bbtop.value,$bbbottom.value);
	};
bbop returns [DataProcessorFilter filter]	
	:	BB^ SPACE! bbleft SPACE! bbright SPACE! bbtop SPACE! bbbottom
	{
		$filter = ArgumentsParser.CreateBoundingBoxFilter($bbleft.value,$bbright.value,$bbtop.value,$bbbottom.value);
	};
bbleft returns [double value]		
	:	LEFT EQUALS NUMBER
	{
		$value = ArgumentsParser.ParseDouble($NUMBER.text);
	};
bbright returns [double value]		
	:	RIGHT EQUALS NUMBER
	{
		$value = ArgumentsParser.ParseDouble($NUMBER.text);
	};
bbtop returns [double value]		
	:	TOP EQUALS NUMBER
	{
		$value = ArgumentsParser.ParseDouble($NUMBER.text);
	};
bbbottom returns [double value]		
	:	BOTTOM EQUALS NUMBER
	{
		$value = ArgumentsParser.ParseDouble($NUMBER.text);
	};
// sorting operator
sortop  returns [DataProcessorFilter filter]
	:	SORT^
	{
		$filter = ArgumentsParser.CreateSortingFilter();
	};
// tokens
BB	:	'--bb' | '--bounding-box';
LEFT	:	'left';
RIGHT	:	'right';
TOP	:	'top';
BOTTOM	:	'bottom';
EQUALS	:	'=';
NUMBER	:	('0'..'9')+'.'('0'..'9')+;
SPACE	:	' ';
LBRACE	:	'(';
RBRACE	:	')';
FILE	:	'file';
//MERGE	:	'--merge'|'--m';
CONNECTIONSTRING
	:	'connectionString' | 'cs';
READXML
	: '--read-xml' | '--rx';	
WRITEXML
	: '--write-xml' | '--wx';	
READXMLCHANGE
	: '--read-xml-change' | '--rxc';	
WRITEXMLCHANGE
	: '--write-xml-change' | '--wxc';
READORACLE
	: '--read-oracle' | '--ro';
WRITEORACLE
	: '--write-oracle' | '--wo';	
READORACLECHANGE
	: '--read-oracle-change' | '--roc';
WRITEORACLECHANGE
	: '--write-oracle-change' | '--woc';	
APPLYORACLECHANGE
	: '--apply-oracle-change' | '--aoc';	
SORT	: '--sort' | '-s';
ARGUMENT 
 	:	'"'(~'"')+'"';