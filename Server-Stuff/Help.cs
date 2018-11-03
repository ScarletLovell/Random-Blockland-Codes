// 2014

package MemberCMDS
{
	function serverCmdHelp(%client, %section, %subSection)
	{
		Commands::sendResult(%client,"Help [" @ %section SPC "---" SPC %subSection @ "]");
		if(%section $= "")
		{
			messageClient(%client,'',"\c6Anthonyrules144's server(s): \c45.9 Beta \c4V8.2");
			messageClient(%client,'',"\c6The 'Help' command is based on sections,");
			messageClient(%client,'',"\c6Say: '/Help [Section]' to view that section!");
			messageClient(%client,'',"\c6Sections: \c4Ranks\c6,  \c4Commands\c6,  \c4Eval\c6, \c4Knives\c6, \c4Members\c6, \c4NewStuff");
			messageClient(%client,'',"http://www.ajsserver.weebly.com \c6Visit this website to check out more of the server!");
			return;
		}
		if(%section !$= "Ranks" && %section !$= "Commands" && %section !$= "Eval" && %section !$= "Knives" && %section !$= "Members" && %section !$= "NewStuff" && %section !$= "GKU")
		{
			messageClient(%client,'',"\c6That doesn't appear to be a section!");
			messageClient(%client,'',"\c6Sections: \c4Ranks\c6,  \c4Commands\c6,  \c4Eval\c6, \c4Knives\c6, \c4Members\c6, \c4Minecraft");
			return;
		}
		
		if(%section $= "Ranks" && %subSection $= "Member")
		{
			messageClient(%client,'',"\c4Member: \c6A member of GKU \c7[Grapple Knifers Unlimited]");
			return;
		}

		if(%section $= "Ranks" && %subSection $= "Respected")
		{
			messageClient(%client,'',"\c4Respected: \c6A good friend of the host, can do \c4very few things");
			return;
		}

		if(%section $= "Ranks" && %subSection $= "VIP")
		{
			messageClient(%client,'',"\c4VIP: \c6This person has donated to the server, can do \c4some 'special' commands");
			return;
		}

		if(%section $= "Ranks" && %subSection $= "CoHost")
		{
			messageClient(%client,'',"\c4CoHost: \c6The host's helper, can do \c4all commands");
			return;
		}

		if(%section $= "Ranks" && %subSection $= "")
		{
			if(%client.isAdmin)
				messageClient(%client,'',"\c6Say \c4/makeRank [name] [rank] \c6to make someone that rank");
			messageClient(%client,'',"\c6Use the command /Help Ranks [RankName]  to see the rank information");
			messageClient(%client,'',"\c6Ranks: \c4Member\c6, \c4Respected\c6, \c4VIP\c6, \c4CoHost");
				return;
		}

		if(%section $= "NewStuff")
		{
			messageClient(%client,'',"\c4There are constantly new things added to the our servers");
			messageClient(%client,'',"\c6One of which, is my new Website: http://www.ajsserverhosting.info ; this website was created by Anthonyrules144 alone.");
			messageClient(%client,'',"\c6There are new knives coming in, so be aware!");
		}

		if(%section $= "Members" && %subSection $= "")
		{
			if(!%client.isMember)
			{
				messageClient(%client,'',"\c6You do not have access to view this section!");
				return;
			}
			
			messageClient(%client,'',"\c6Members can do certain things in KnifeTDM, but not in Speedykart;");
			messageClient(%client,'',"\c6Mebers can \c4/changeTeam\c6");
			messageClient(%client,'',"\c6Members can do certain things in KnifeTDM, but not in Speedykart;");
		}

		if(%section $= "Knives" && %subSection $= "")
		{
			messageClient(%client,'',"\c6You earn knives every 10 levels, starting at 10.");
			messageClient(%client,'',"\c6There are currently 7 knives, maybe more in the future.");
			messageClient(%client,'',"\c6You can equip a knife by saying /knife to see your knives and /knife [#] to equipt the #'d knife.");
		}

		if(%section $= "Eval" && %subSection $= "")
		{
			messageClient(%client,'',"\c4Eval\c6: The access to the console from in-game.");
			messageClient(%client,'',"\c6To use eval you must be granted eval by the host.");
			
			if(%client.hasEval)
				messageClient(%client,'',"\c6 Use the key \c4$ \c6in-game-chat to start using Eval.");
		}

		if(%section $= "Commands" && %subSection $= "")
		{
			messageClient(%client,'',"\c6Say \c4/CommandHere \c6to do that command!");
			if(%client.isSuperAdmin)
			{
				messageClient(%client,'',"\c4SA: \c6Your commands are:");
				messageClient(%client,'',"\c4Help Lol SetPrefix SetSuffix RIP Warn Forcekill Changeteam ResetAwards Setlevel GiveAward MC AC");
				return;
			}

			if(%client.isAdmin)
			{
				messageClient(%client,'',"\c4ADMIN: \c6Your commands are:");
				messageClient(%client,'',"\c4Help Lol SetPrefix SetSuffix RIP Warn Forcekill Changeteam Setlevel MC AC");
				return;
			}

			if(%client.isModerator)
			{
				messageClient(%client,'',"\c4MOD: \c6Your commands are:");
				messageClient(%client,'',"\c4Help Lol SetPrefix SetSuffix RIP Warn Forcekill Changeteam MC");
				return;
			}

			if(%client.isVIP)
			{
				messageClient(%client,'',"\c4VIP: \c6Your commands are:");
				messageClient(%client,'',"\c4Help Lol SetPrefix SetSuffix RIP Changeteam Slap RCToggle");
				return;
			}
			
			messageClient(%client,'',"\c4GUEST: \c6Your commands are:");
			messageClient(%client,'',"\c4Help Lol SetPrefix SetSuffix");
		}
	}
};
activatePackage(MemberCMDS);