
from list_types import Package

overview = [
    #=========================================================================

    Package ("Infrastructure", "8.1", [
         ( "Definitions"                    , "attr. str. 53; XSD str. 54"        ),
         ( "Import"                         , "attr. str. 54; XSD str. 55"        ),
      ]),

    #=========================================================================

    Package ("Foundation", "8.2", [

         ( "BaseElement"                    , "attr. str. 56; XSD str. 64"        ),
         # ( "BaseElementWithMixedContent"    , "není v obj. modelu; XSD str. 64"  ),

         ( "Documentation"                  , "attr. str. 56; XSD str. 64-65"     ),

         ( "Extension"                      , "attr. str. 58; XSD str. 60"        ),
         # ( "ExtensionElements"              , "není v obj. modelu; XSD str. 64"  ),
         ( "ExtensionDefinition"            , "attr. str. 59"                     ),
         ( "ExtensionAttributeDefinition"   , "attr. str. 59"                     ),
         ( "ExtensionAttributeValue"        , "attr. str. 59"                     ),
         
         ( "Relationship"                   , "attr. str. 63; XSD str. 65"        ),
         ( "RelationshipDirection"          , "XSD str. 65"                       ),
        
         ( "RootElement"                    , "text str. 64; XSD str. 65"         ),
      ]),

    #=========================================================================

    Package ("Artifacts", "8.3.1", [

         ( "Artifact"                       , "text str. 66; XSD str. 72"         ),
         ( "Association"                    , "attr. str. 68; XSD str. 72"        ),
         ( "AssociationDirection"           , "XSD str. 72"                       ),
         ( "TextAnnotation"                 , "attr. str. 72; XSD str. 73"        ),
         # ( "Text"                           , "není v obj. modelu; XSD str. 74"  ),
         ( "Group"                          , "attr. str. 70; XSD str. 73"        ),
         ( "Category"                       , "attr. str. 71; XSD str. 73"        ),
         ( "CategoryValue"                  , "attr. str. 71; XSD str. 73"        ),
      ]),

    #=========================================================================

    Package ("Common", "8.3.2 - 8.3.13, 10.2.6", [

         ( "FlowElement"                     , "attr. str. 88; XSD str. 101"      ),
         ( "FlowElementsContainer"           , "attr. str. 90; nemá XSD"          ),
         ( "FlowNode"                        , "attr. str. 100; XSD str. 101"     ),
         ( "SequenceFlow"                    , "attr. str. 99; XSD str. 103"      ),

         ( "Expression"                      , "text str. 85; XSD str. 100"       ),
         ( "FormalExpression"                , "attr str. 86; XSD str. 101"       ),

         ( "ItemDefinition"                  , "attr. str. 92; XSD str. 102"      ),
         ( "ItemKind"                        , "XSD str. 102"                     ),

         ( "Error"                           , "attr. str. 82; XSD str. 100"      ),

         ( "Escalation"                      , "attr. str. 83; XSD str. 100"      ),

         ( "Message"                         , "attr. str. 95; XSD str. 102"      ),

         ( "Resource"                        , "attr. str. 96; XSD str. 102"      ),
         ( "ResourceParameter"               , "attr. str. 97; XSD str. 103"      ),

         ( "CallableElement"                 , "attr. str. 187; XSD str. 197"     ),
         ( "InputOutputBinding"              , "attr. str. 187; XSD str. 102"     ),

         ( "CorrelationKey"                  , "attr. str. 77; XSD str. 79"       ),
         ( "CorrelationProperty"             , "attr. str. 77; XSD str. 79-80"    ),
         ( "CorrelationPropertyRetrievalExpression" , "attr. str. 78; XSD str. 80" ),
         ( "CorrelationSubscription"         , "attr. str. 78; XSD str. 80"       ),
         ( "CorrelationPropertyBinding"      , "attr. str. 79; XSD str. 80"       ),

      ]),

    #=========================================================================

    Package( "Service", "8.4", [

         ( "Interface"                       , "attr. str. 105; XSD str. 106"     ),
         ( "Operation"                       , "attr. str. 106; XSD str. 106"     ),
         ( "EndPoint"                        , "attr. str. 105; XSD str. 107"     ),
      ]),

    #=========================================================================

    Package( "Activities", "10.2", [

         ( "Activity"                        , "attr. str. 152-153; XSD str. 195" ),

         ( "ResourceRole"                    , "attr. str. 155; XSD str. 200"     ),
         ( "ResourceAssignmentExpression"    , "attr. str. 155"                   ),
         ( "ResourceParameterBinding"        , "attr. str. 156"                   ),

         ( "Task"                            , "text str. 156; XSD str. 202"      ),
         ( "ServiceTask"                     , "attr. str. 159; XSD str. 201"     ),
         ( "SendTask"                        , "attr. str. 161; XSD str. 201"     ),
         ( "ReceiveTask"                     , "attr. str. 162; XSD str. 200"     ),
         ( "UserTask"                        , "attr. str. 167; XSD str. 169"     ),
         ( "ManualTask"                      , "text str. 163; XSD str. 168"      ),
         ( "BusinessRuleTask"                , "attr. str. 164; XSD str. 196"     ),
         ( "ScriptTask"                      , "attr. str. 165; XSD str. 201"     ),

         # ( "Script"                          , "není v obj. modelu; XSD str. 201" ),
         ( "Rendering"                       , "XSD str. 169"                     ),
         # ( "Implementation"                  , "XSD str. 169"                    ),

         ( "SubProcess"                      , "attr. str. 176; XSD str. 202"     ),
         ( "Transaction"                     , "attr. str. 180; XSD str. 203"     ),
         # ( "TransactionMethod"               , "XSD str. 203"                    ),
         ( "AdHocSubProcess"                 , "attr. str. 181; XSD str. 196"     ),
         ( "AdHocOrdering"                   , "XSD str. 196"                     ),

         ( "CallActivity"                    , "attr. str. 186; XSD str. 197"     ),

         ( "GlobalTask"                      , "attr. str. 188; XSD str. 198, 313" ),
         ( "GlobalBusinessRuleTask"          , "XSD str. 197"                     ),
         ( "GlobalScriptTask"                , "XSD str. 198"                     ),

         ( "LoopCharacteristics"             , "text str. 189-190; XSD str. 198"  ),
         ( "StandardLoopCharacteristics"     , "attr. str. 191; XSD str. 202"     ),
         ( "MultiInstanceLoopCharacteristics" , "attr. str. 192-193; XSD str. 199" ),
         # ( "MultiInstanceFlowCondition"      , "XSD str. 196"                    ),
         ( "MultiInstanceBehavior"           , None                               ),
         ( "ComplexBehaviorDefinition"       , "attr. str. 195"                   ),
      ]),

    #=========================================================================

    Package( "Data", "10.3", [

         ( "ItemAwareElement"                , "attr. str. 204"                   ),
         ( "DataObject"                      , "attr. str. 206; XSD str. 230"     ),
         ( "DataObjectReference"             , "attr. str. 206"                   ),
         ( "DataState"                       , "attr. str. 206; XSD str. 230"     ),
         ( "DataStore"                       , "attr. str. 209"                   ),
         ( "DataStoreReference"              , "attr. str. 210"                   ),
         ( "Property"                        , "attr. str. 211; XSD str. 233"     ),

         ( "InputOutputSpecification"        , "attr. str. 213; XSD str. 231"     ),
         ( "DataInput"                       , "attr. str. 215; XSD str. 229"     ),
         ( "DataOutput"                      , "attr. str. 217; XSD str. 231"     ),
         ( "InputSet"                        , "attr. str. 219; XSD str. 232"     ),
         ( "OutputSet"                       , "attr. str. 221; XSD str. 232"     ),

         ( "DataAssociation"                 , "attr. str. 223; XSD str. 229"     ),
         ( "Assignment"                      , "attr. str. 224; XSD str. 229"     ),
         ( "DataInputAssociation"            , "text str. 224; XSD str. 230"      ),
         ( "DataOutputAssociation"           , "text str. 224; XSD str. 231"      ),
      ]),

    #=========================================================================

    Package( "Events", "10.4", [

         ( "Event"                           , "attr. str. 236; XSD str. 284"     ),
         ( "CatchEvent"                      , "attr. str. 236-237; XSD str. 282" ),
         ( "ThrowEvent"                      , "attr. str. 237-238; XSD str. 286" ),
         ( "StartEvent"                      , "attr. str. 245; XSD str. 286"     ),
         ( "EndEvent"                        , "text str. 246-249"                ),
         ( "ImplicitThrowEvent"              , "text str. 238; XSD str. 284"      ),
         ( "IntermediateThrowEvent"          , "XSD str. 284"                     ),
         ( "IntermediateCatchEvent"          , "XSD str. 284"                     ),
         ( "BoundaryEvent"                   , "attr. str. 258; XSD str. 282"     ),

         ( "EventDefinition"                 , "text str. 260; XSD str. 284"      ),
         ( "CancelEventDefinition"           , "text str. 263; XSD str. 282, 283" ),
         # ( "CompensationEventDefinition"     , "attr. str. 264; XSD str. 283"    ),
         ( "CompensateEventDefinition"       , "attr. str. 264; XSD str. 283"     ),
         ( "ConditionalEventDefinition"      , "attr. str. 265; XSD str. 283"     ),
         ( "ErrorEventDefinition"            , "attr. str. 266; XSD str. 283"     ),
         ( "EscalationEventDefinition"       , "attr. str. 267; XSD str. 283"     ),
         ( "LinkEventDefinition"             , "attr. str. 270; XSD str. 285"     ),
         ( "MessageEventDefinition"          , "attr. str. 271; XSD str. 285"     ),
         ( "SignalEventDefinition"           , "attr. str. 273; XSD str. 286"     ),
         ( "Signal"                          , "text str. 273; XSD str. 285"      ),
         ( "TerminateEventDefinition"        , "text str. 273-274; XSD str. 286"  ),
         ( "TimerEventDefinition"            , "attr. str. 274; XSD str. 287"     ),
      ]),

    #=========================================================================

    Package( "Gateways", "10.5", [

         ( "Gateway"                         , "text str. 287-290; XSD str. 301-302" ),
         ( "GatewayDirection"                , "XSD str. 302"                     ),
         ( "ExclusiveGateway"                , "attr. str. 292; XSD str. 301"     ),
         ( "InclusiveGateway"                , "attr. str. 293; XSD str. 302"     ),
         ( "ParallelGateway"                 , "text str. 293-295; XSD str. 302"  ),
         ( "ComplexGateway"                  , "attr. str. 296; XSD str. 301"     ),
         ( "EventBasedGateway"               , "attr. str. 300; XSD str. 301"     ),
         ( "EventBasedGatewayType"           , "XSD str. 301"                     ),
      ]),

    #=========================================================================

    Package( "Process", "10-intro, 10.7", [

         ( "Process"                         , "attr. str. 147-149; XSD str. 312" ),
         ( "ProcessType"                     , "XSD str. 312-313"                 ),
         ( "LaneSet"                         , "attr. str. 308; XSD str. 313-314" ),
         ( "Lane"                            , "attr. str. 309; XSD str. 313"     ),
         ( "Auditing"                        , "text str. 311; XSD str. 313"      ),
         ( "Monitoring"                      , "text str. 311-312; XSD str. 314"  ),
         ( "Performer"                       , "text str. 156; XSD str. 314"      ),
      ]),

    #=========================================================================

    Package( "HumanInteraction", "10.2.4", [

         ( "HumanPerformer"                  , "XSD str. 169"                     ),
         ( "PotentialOwner"                  , "XSD str. 169"                     ),
         ( "GlobalUserTask"                  , None                               ),
         ( "GlobalManualTask"                , None                               ),
      ]),

    #=========================================================================

    Package( "Collaboration", "9.1 - 9.3", [

         ( "Collaboration"                   , "attr. str. 110-111; XSD str. 140" ),
         ( "InteractionNode"                 , "text str. 123"                    ),
         ( "Participant"                     , "attr. str. 116; XSD str. 142"     ),
         ( "ParticipantMultiplicity"         , "attr. str. 118; XSD str. 143"     ),
         ( "ParticipantAssociation"          , "attr. str. 119; XSD str. 143"     ),
         ( "MessageFlow"                     , "attr. str. 123; XSD str. 142"     ),
         ( "MessageFlowAssociation"          , "attr. str. 124; XSD str. 142"     ),
         ( "PartnerEntity"                   , "attr. str. 116; XSD str. 143"     ),
         ( "PartnerRole"                     , "attr. str. 117; XSD str. 143"     ),
      ]),

    #=========================================================================

    Package( "Conversations", "9.4", [

         ( "ConversationNode"                , "attr. str. 130; XSD str. 141"     ),
         ( "Conversation"                    , "text str. 130; XSD str. 140"      ),
         ( "SubConversation"                 , "attr. str. 131; XSD str. 144"     ),
         ( "CallConversation"                , "attr. str. 132; XSD str. 139"     ),
         ( "GlobalConversation"              , "text str. 132; XSD str. 141-142"  ),
         ( "ConversationLink"                , "attr. str. 134; XSD str. 140"     ),
         ( "ConversationAssociation"         , "attr. str. 136; XSD str. 140"     ),
      ]),

    #=========================================================================

    Package( "Choreography", "11", [

         ( "Choreography"                    , "XSD str. 364"        ),
         ( "GlobalChoreographyTask"          , "attr. str. 335; XSD str. 365"     ),
      ]),

    #=========================================================================

    Package( "ChoreographyActivities", "11", [

         ( "ChoreographyActivity"            , "attr. str. 322; XSD str. 365"     ),
         ( "ChoreographyLoopType"            , "XSD str. 365"                     ),
         ( "ChoreographyTask"                , "attr. str. 328; XSD str. 365"     ),
         ( "SubChoreography"                 , "attr. str. 332; XSD str. 366"     ),
         ( "CallChoreography"                , "attr. str. 335; XSD str. 366"     ),
      ]),

    #=========================================================================
]
