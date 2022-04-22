using Godot;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

[JsonObject(MemberSerialization.OptIn)]
public class SetStaticDefaultsNode : BlueprintNode
{
	[JsonProperty]
	public string DisplayName
	{
		get { return DisplayNameInput.Text; }
		set
		{
			DisplayNameInput.Text = value;
		}
	}

	[JsonProperty]
	public string Tooltip
	{
		get { return TooltipEdit.Text; }
		set
		{
			TooltipEdit.Text = value;
		}
	}

	public override void PostPopulate() => StringInputTextChanged(true);

	LineEdit DisplayNameInput;
	Panel TooltipBox;
	TextEdit TooltipEdit;
	float TooltipBoxBaseHeight;

	public override void _Ready()
	{
		base._Ready();
		DisplayNameInput = GetNode<LineEdit>("VBoxContainer/DisplayNameBox/DisplayNameInput");
		TooltipBox = GetNode<Panel>("VBoxContainer/TooltipBox");
		TooltipEdit = TooltipBox.GetNode<TextEdit>("StringInput");
		TooltipBoxBaseHeight = TooltipBox.RectSize.y;
	}

	private void _on_StringInput_text_changed() => StringInputTextChanged();

	private async void StringInputTextChanged(bool awaitIdleFrames = false)
	{
		if (awaitIdleFrames)
		{
			// Hacky fix
			await ToSignal(GetTree(), "idle_frame");
			await ToSignal(GetTree(), "idle_frame");
			FinishedLoading = true;
		}
		int lines = TooltipEdit.Text.Split("\n").Length;
		int linesWithWrapping = lines - 1;
		for (var i = 0; i < lines; i++)
		{
			linesWithWrapping += TooltipEdit.GetLineWrapCount(i);
		}
		TooltipEdit.RectSize = new Vector2(TooltipEdit.RectSize.x, 21f + 18f * linesWithWrapping);
		TooltipBox.RectMinSize = new Vector2(TooltipBox.RectSize.x, TooltipBoxBaseHeight + 18f * linesWithWrapping);
	}

	private List<SyntaxAnnotation> tooltipAnnotations = new List<SyntaxAnnotation>();

	public override void OnExport(ref ClassDeclarationSyntax mainClassDeclaration)
	{
		// DisplayName.SetDefault
		var displayNameSetDefaultArgument = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(DisplayName)));
		var displayNameSetDefaultArgumentList = SyntaxFactory.SeparatedList(new[] { displayNameSetDefaultArgument });
		var displayNameSetDefaultSyntax = SyntaxFactory.ExpressionStatement(
			SyntaxFactory.InvocationExpression(
				SyntaxFactory.MemberAccessExpression(
					SyntaxKind.SimpleMemberAccessExpression,
					SyntaxFactory.IdentifierName("DisplayName"),
					SyntaxFactory.IdentifierName("SetDefault")),
				SyntaxFactory.ArgumentList(displayNameSetDefaultArgumentList)));

		// Tooltip.SetDefault
		string[] tooltipLines = Tooltip.Split("\n");
		ExpressionSyntax[] tooltipExpression = new ExpressionSyntax[tooltipLines.Length];
		for (var i = 0; i < tooltipLines.Length; i++)
		{
			tooltipExpression[i] = SyntaxFactory.LiteralExpression(
				SyntaxKind.StringLiteralExpression,
				SyntaxFactory.Literal(i != 0 ? "\n" + tooltipLines[i] : tooltipLines[i]));
		}
		var expr = tooltipExpression[0];
		foreach (var line in tooltipExpression.Skip(1))
		{
			var syntaxAnnotation = new SyntaxAnnotation();
			tooltipAnnotations.Add(syntaxAnnotation);
			expr = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, expr, line)
				.WithAdditionalAnnotations(syntaxAnnotation);
		}

		var tooltipSetDefaultArgument = SyntaxFactory.Argument(expr);
		var tooltipSetDefaultArgumentList = SyntaxFactory.SeparatedList(new[] { tooltipSetDefaultArgument });
		var tooltipSetDefaultSyntax = SyntaxFactory.ExpressionStatement(
			SyntaxFactory.InvocationExpression(
				SyntaxFactory.MemberAccessExpression(
					SyntaxKind.SimpleMemberAccessExpression,
					SyntaxFactory.IdentifierName("Tooltip"),
					SyntaxFactory.IdentifierName("SetDefault")),
				SyntaxFactory.ArgumentList(tooltipSetDefaultArgumentList)));

		// Create a method
		var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "SetStaticDefaults")
			.AddModifiers(
				SyntaxFactory.Token(SyntaxKind.PublicKeyword),
				SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
			.WithBody(SyntaxFactory.Block(displayNameSetDefaultSyntax, tooltipSetDefaultSyntax));

		// Add the field, the property and method to the class.
		mainClassDeclaration = mainClassDeclaration.AddMembers(methodDeclaration);
	}

	public override void PostExport(ref CompilationUnitSyntax compilationUnit)
	{
		for (var i = 0; i < tooltipAnnotations.Count; i++)
		{
			var changedClass = compilationUnit.DescendantNodes().OfType<BinaryExpressionSyntax>()
			.Where(n => n.HasAnnotation(tooltipAnnotations[i])).Single();
			SyntaxToken plusToken = SyntaxFactory.Token(SyntaxKind.PlusToken);
			SyntaxTriviaList leadingWhiteSpace = plusToken.LeadingTrivia;
			SyntaxTrivia tabWhitespace = SyntaxFactory.Whitespace("                               ");
			SyntaxTrivia endOfLine = SyntaxFactory.EndOfLine("\n");
			plusToken = plusToken.WithLeadingTrivia(leadingWhiteSpace.Insert(0, tabWhitespace).Insert(0, endOfLine));
			plusToken = plusToken.WithTrailingTrivia(SyntaxFactory.Whitespace(" "));
			var newChangedClass = changedClass.ReplaceToken(changedClass.ChildTokens().First(), plusToken);
			compilationUnit = compilationUnit.ReplaceNode(changedClass, newChangedClass);
		}
		tooltipAnnotations.Clear();
	}
}
