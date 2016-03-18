﻿using System;
using DbUp.Tests.TestInfrastructure;
using NUnit.Framework;
using Shouldly;

namespace DbUp.Tests.Support.MySql
{
    [TestFixture]
    public class MySqlSupportTests
    {
        [Test]
        public void CanHandleDelimiter()
        {
            var logger = new CaptureLogsLogger();
            var recordingDbConnection = new RecordingDbConnection(logger, true, "schemaversions");
            var upgrader = DeployChanges.To
                .MySqlDatabase(string.Empty)
                .OverrideConnectionFactory(recordingDbConnection)
                .LogTo(logger)
                .WithScript("Script0003", @"USE `test`;
DROP procedure IF EXISTS `testSproc`;

DELIMITER $$

USE `test`$$
CREATE PROCEDURE `testSproc`(
        IN   ssn                    VARCHAR(32)
     )
BEGIN 

    SELECT id      
    FROM   customer as c
    WHERE  c.ssn = ssn ; 

END$$").Build();

            var result = upgrader.PerformUpgrade();

            result.Successful.ShouldBe(true);
            logger.Log.ShouldMatchApproved(b => b.WithScrubber(Scrubbers.ScrubDates));
        }
    }
}