using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using PixelCrushers.DialogueSystem;

public class ContinueButtonResumeTimeline : StandardUIContinueButtonFastForward
{
    public override void OnFastForward()
    {
        if ((typewriterEffect != null) && typewriterEffect.isPlaying)
        {
            typewriterEffect.Stop();
        }
        else
        {
            ResumeTimeline();
        }
    }

    private void ResumeTimeline()
    {
        // Assumes current line's Sequence start with "Timeline(speed, TIMELINENAME, 0)"
        // where "TIMELINENAME" is the name of a PlayableDirector GameObject.
        var sequence = DialogueManager.currentConversationState.subtitle.sequence;
        if (sequence.Contains("Timeline(speed"))
        {
            // Get the timeline name:
            var pos = sequence.IndexOf(',');
            var s = sequence.Substring(pos + 1);
            pos = s.IndexOf(',');
            s = s.Substring(0, pos).Trim();
            // Resume the timeline:
            var subject = SequencerTools.FindSpecifier(s);
            var playableDirector = (subject != null) ? subject.GetComponent<PlayableDirector>() : null;
            if (playableDirector != null)
            {
                if (DialogueDebug.logInfo) Debug.Log($"Dialogue System: Resuming timeline {playableDirector}", playableDirector);
                playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
                //---DialogueManager.standardDialogueUI.HideSubtitle(DialogueManager.currentConversationState.subtitle);
                return;
            }
        }
        // If we don't resume the timeline, just advance the conversation:
        base.OnFastForward();
    }


}
