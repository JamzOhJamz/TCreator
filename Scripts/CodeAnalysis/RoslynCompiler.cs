using Godot;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

public class RoslynCompiler : Node
{
    public void Export()
    {
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Add Terraria.ModLoader using statement: (using Terraria.ModLoader)
        compilationUnit = compilationUnit.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Terraria.ModLoader")));

        // Create a namespace: (namespace TCreator)
        var @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("TCreator")).NormalizeWhitespace();

        // Create a class: (public class ItemBlueprint : ModItem)
        var classDeclaration = SyntaxFactory.ClassDeclaration("ItemBlueprint")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddBaseListTypes(
            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("ModItem")));

        foreach (var node in TCreator.WorkspaceNodes)
            node.OnExport(ref classDeclaration);

        // Add the class to the namespace.
        @namespace = @namespace.AddMembers(classDeclaration);

        compilationUnit = compilationUnit.AddMembers(@namespace);

        // Normalize whitespace.
        compilationUnit = compilationUnit.NormalizeWhitespace();

        foreach (var node in TCreator.WorkspaceNodes)
            node.PostExport(ref compilationUnit);

        // Output new code to the console.
        GD.Print(compilationUnit.ToFullString());
    }

    static void PrintSyntaxTree(SyntaxNode node)
    {
        if (node != null)
        {
            foreach (var item in node.ChildTokens())
            {
                GD.Print(item);
            }
            GD.Print("");
            foreach (SyntaxNode child in node.ChildNodes())
                PrintSyntaxTree(child);
        }
    }

}