﻿/*
 * Tfs2Slack - http://github.com/kria/Tfs2Slack
 * 
 * Copyright (C) 2014 Kristian Adrup
 * 
 * This file is part of Tfs2Slack.
 * 
 * Tfs2Slack is free software: you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version. See included file COPYING for details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Git.Common;

namespace DevCore.Tfs2Slack.Notifications
{
    class PullRequestStatusUpdateNotification : BaseNotification
    {
        protected readonly static Configuration.TextElement text = Configuration.Tfs2SlackSection.Instance.Text;
        protected readonly static Configuration.SettingsElement settings = Configuration.Tfs2SlackSection.Instance.Settings;

        public PullRequestStatus Status { get; set; } 
        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string ProjectName { get; set; }
        public string RepoUri { get; set; }
        public string RepoName { get; set; }
        public int PrId { get; set; }
        public string PrUrl { get; set; }
        public string PrTitle { get; set; }
        public string Action
        {
            get
            {
                switch (Status)
                {
                    case PullRequestStatus.Abandoned: return text.Abandoned;
                    case PullRequestStatus.Active: return text.Reactivated;
                    case PullRequestStatus.Completed: return text.Completed;
                    default:
                        return String.Format("updated status to {0} for", Status.ToString());
                }
            }
        }
        public string UserName
        {
            get { return settings.StripUserDomain ? Utils.StripDomain(UniqueName) : UniqueName; }
        }

        public override IList<string> ToMessage(Configuration.BotElement bot)
        {
            return new[] { text.PullRequestStatusUpdateFormat.FormatWith(this) };
        }

        public override bool IsMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.PullRequestStatusUpdate)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && RepoName.IsMatchOrNoPattern(r.GitRepository));

            if (rule != null) return rule.Notify;

            return false;
        }
    }
}